using System;
using System.Text;
using System.IO;
using Guineu.Data.Dbf;
using Guineu.Util;

// TODO: Extract interfaces from classes. Other providers can implement these interfaces

namespace Guineu.Data.Engines.Dbf
{
	public class DbfTable : IDisposable
	{
		Stream dataStream;
		bool disposed;
		readonly TableHeader tableHeader;
		readonly LockManager lockMgr;
		internal IndexFile Cdx;
		readonly MemoFile memo;


		readonly String fileName;
		readonly Boolean exclusive;
		//===================================================================================
		/// <summary>
		/// Opens the specified table
		/// </summary>
		/// <param name="file">Full path to the DBF file</param>
		/// <param name="openExclusive">When true table is opened exclusively</param>
		public DbfTable(string file, Boolean openExclusive)
		{
			fileName = file;
			exclusive = openExclusive;
			OpenTable();
			tableHeader = new TableHeader(dataStream);
			lockMgr = new LockManager(this);
			if (EnumUtil.IsSet((Int32)tableHeader.flags, (Int32)TableFlags.HasCDX))
			{
				Cdx = new IndexFile(Path.ChangeExtension(file, "CDX"));
			}
			if (EnumUtil.IsSet((Int32)tableHeader.flags, (Int32)TableFlags.HasMemo))
				memo = new MemoFile(file, openExclusive);
		}

		void OpenTable()
		{
			if (exclusive)
				dataStream = GuineuInstance.FileMgr.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			else
				dataStream = GuineuInstance.FileMgr.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
		}

		public Byte[] ReadMemoField(Int32 block)
		{
			return memo.Read(block);
		}

		public Int32 WriteMemoField(Byte[] data, Int32 block)
		{
			return memo.Write(data, block);
		}

		internal MemoFile MemoFile
		{
			get { return memo; }
		}


		//===================================================================================
		internal FieldDefinition Definition
		{
			get { return tableHeader.GetDefinition(); }
		}

		//===================================================================================
		internal TableHeader Header
		{
			get { return tableHeader; }
		}

		//===================================================================================
		internal Byte[] ReadRawRecord(Int64 recNo)
		{
			Byte[] buf = GetRawRecordBuffer();
			ReadRawRecord(buf, recNo);
			return buf;
		}



		//===================================================================================
		internal Byte[] BlankRawRecord()
		{
			// (...) need to fill record depending on data type. Some use 0x20, other 0x00
			Byte[] buf = GetRawRecordBuffer();


			// IField
			for (int b = 0; b < buf.Length; b++)
			{
				buf[b] = 0x20;
			}
			return buf;
		}

		//===================================================================================
		internal void WriteRawRecord(Int64 recNo, Byte[] buf)
		{
			UpdateRecordCount();
			if (lockMgr.LockRecord(recNo))
			{
				IoOperation(() =>
											{
												dataStream.Seek(
													tableHeader.GetHeaderLength() + (recNo - 1) * tableHeader.GetRecordSize(),
													SeekOrigin.Begin
													);
												dataStream.Write(buf, 0, buf.Length);
												if (recNo == 1)
													Console.Write("");
												dataStream.Flush();
											}
					);
				lockMgr.UnlockRecord(recNo);
			}
		}

		//===================================================================================
		private void UpdateRecordCount()
		{
			IoOperation(() => tableHeader.RefreshHeader(dataStream));
		}

		//===================================================================================
		Byte[] GetRawRecordBuffer()
		{
			var buf = new Byte[tableHeader.GetRecordSize()];
			return buf;
		}

		//===================================================================================
		void ReadRawRecord(Byte[] buf, Int64 recNo)
		{
			Int32 size = tableHeader.GetRecordSize();

			if (recNo < 1 || recNo > tableHeader.GetRecCount())
				throw new ArgumentOutOfRangeException("recNo");

			// TODO: Reload record count, when recno>reccount
			IoOperation(() =>
										{
											dataStream.Seek(tableHeader.GetHeaderLength() + (recNo - 1) * size, SeekOrigin.Begin);
											dataStream.Read(buf, 0, size);
										});
		}

		void IoOperation(Action operation)
		{
			try
			{
				operation();
			}
			catch (IOException)
			{
				OpenTable();
				operation();
			}
		}

		//===================================================================================
		internal Encoding GetEncoding()
		{
			return tableHeader.GetEncoder();
		}


		internal Record GetRecord(Int64 recNo)
		{
			Byte[] content;
			if (recNo == 0)
				content = BlankRawRecord();
			else if (recNo == tableHeader.GetRecCount() + 1)
				content = BlankRawRecord();
			else
				content = ReadRawRecord(recNo);
			return new Record(recNo, Header.GetDefinition(), MemoFile, GetEncoding(), content, new FieldFactory());
		}

		//===================================================================================
		internal Int64 Append()
		{
			if (!lockMgr.LockHeader())
				return 0;

			IoOperation(() => tableHeader.RefreshHeader(dataStream));
			Int64 cnt = tableHeader.GetRecCount() + 1;
			Int32 recSize = tableHeader.GetRecordSize();

			Int64 newSize = tableHeader.GetHeaderLength() + cnt * recSize;
			SetStreamLength(newSize);
			using (var w = new BinaryWriter(new Core.NonClosingStream(dataStream)))
				tableHeader.UpdateRecordCount(w, cnt);
			Byte[] content = BlankRawRecord();
			GetRecord(0).Gather(content);
			WriteRawRecord(cnt, content);


			dataStream.Flush();

			lockMgr.UnlockHeader();
			return cnt;
		}

		private void SetStreamLength(Int64 newSize)
		{
			try
			{
				IoOperation(() => dataStream.SetLength(newSize));
			}
			catch (NotSupportedException)
			{
				// In the Compact emulator this line can cause an error. This is a known
				// problem that hasn't been fixed yet.
				// http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=796830&SiteID=1
			}

		}
		//===================================================================================
		internal void Zap()
		{
			if (lockMgr.LockHeader())
			{
				tableHeader.RefreshHeader(dataStream);
				Int64 newSize = tableHeader.GetHeaderLength();
				SetStreamLength(newSize);

				using (var w = new BinaryWriter(new Core.NonClosingStream(dataStream)))
					tableHeader.UpdateRecordCount(w, 0);

				dataStream.Flush();
				tableHeader.RefreshHeader(dataStream);

				lockMgr.UnlockHeader();
			}
		}

		internal Stream Stream
		{
			get { return dataStream; }
		}

		#region IDispose pattern

		virtual protected void Dispose(bool disposing)
		{
			if (!disposed && disposing)
			{
				disposed = true;
				if (dataStream != null)
					dataStream.Close();
				if (memo != null)
					memo.Close();
				if(Cdx != null)
					Cdx.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		internal void Close()
		{
			Dispose();
		}

		public long GetRecCount()
		{
			IoOperation(() => tableHeader.RefreshHeader(dataStream));
			return tableHeader.GetRecCount();
		}
	}
}