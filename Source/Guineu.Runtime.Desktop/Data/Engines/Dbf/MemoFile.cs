using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Core;

namespace Guineu.Data.Dbf
{
	class MemoFile : IDisposable
	{
		Stream stream;
		Object streamLock;
		BinaryReader reader;
		BinaryWriter writer;

		Int16 blockSize;

		public MemoFile(String tableFileName, Boolean exclusive)
		{
			streamLock = new Object();
			OpenMemoFile(tableFileName, exclusive);
			this.reader = new BinaryReader(this.stream, GuineuInstance.CurrentCp);
			this.writer = new BinaryWriter(this.stream, GuineuInstance.CurrentCp);
			ReadHeaderBlock();
		}


		#region Private methods
		void ReadHeaderBlock()
		{
			lock (streamLock)
			{
				stream.Position = 6;
				blockSize = (Int16)(reader.ReadByte() * 256 + reader.ReadByte());
			}
		}
		void OpenMemoFile(String tableFileName, Boolean exclusive)
		{
			FileShare mode;
			if (exclusive)
				mode = FileShare.None;
			else
				mode = FileShare.ReadWrite;
			stream = GuineuInstance.FileMgr.Open(
				GetMemoFileName(tableFileName),
				FileMode.Open,
				FileAccess.ReadWrite,
				mode);
		}
		String GetMemoFileName(String fileName)
		{
			String extension = Path.GetExtension(fileName).ToUpper();
			switch (extension)
			{
				case ".DBF":
					return Path.ChangeExtension(fileName, "FPT");
				case ".SCX":
					return Path.ChangeExtension(fileName, "SCT");
				case ".VCX":
					return Path.ChangeExtension(fileName, "VCT");
				case ".PJX":
					return Path.ChangeExtension(fileName, "PJT");
				case ".MNX":
					return Path.ChangeExtension(fileName, "MNT");
				case ".LBX":
					return Path.ChangeExtension(fileName, "LBT");
				case ".FRX":
					return Path.ChangeExtension(fileName, "FRT");
				case ".DBC":
					return Path.ChangeExtension(fileName, "DCT");
				default:
					return Path.ChangeExtension(fileName, "FPT");
			}
		}
		#endregion

		public void Close()
		{
			Dispose();
		}

		public Byte[] Read(Int32 block)
		{
			lock (streamLock)
			{
				stream.Position = block * blockSize + 4;
				Int32 length = reader.ReadByte() * 256 * 256 * 256 + reader.ReadByte() * 256 * 256 + reader.ReadByte() * 256 + reader.ReadByte();
				byte[] data = reader.ReadBytes(length);
				return data;
			}
		}

		Int32 ReadInt32()
		{
			Int32 value = reader.ReadByte() * 256 * 256 * 256 + reader.ReadByte() * 256 * 256 + reader.ReadByte() * 256 + reader.ReadByte();
			return value;
		}

		void Write(Int32 value)
		{
			Byte[] data = new Byte[4];
			using (MemoryStream ms = new MemoryStream(data))
			using (BinaryWriter bw = new BinaryWriter(ms))
				bw.Write(value);
			writer.Write(data[3]);
			writer.Write(data[2]);
			writer.Write(data[1]);
			writer.Write(data[0]);
		}

		public Int32 Write(Byte[] data, Int32 block)
		{
			// In the first iteration we always add the block to the end. Later we can use 
			// the block to check if the string fits into the existing block.
			lock (streamLock)
			{
				Int32 newBlock = (((Int32)stream.Length)) / blockSize;

				// Write new record
				stream.Seek(0, SeekOrigin.End);
				writer.Write(new Byte[] { 0, 0, 0, 1 });
				Write(data.Length);
				writer.Write(data);

				Int32 inLastBlock = (data.Length + 8) % blockSize;
				if (inLastBlock != 0)
				{
					Byte[] empty = new Byte[blockSize - inLastBlock];
					writer.Write(empty);
				}

				//Int32 lastBlock = (((Int32)stream.Length)) / blockSize;
				stream.Seek(0, SeekOrigin.Begin);
				//Write(lastBlock+1);
				Write((Int32)newBlock+1);

				return newBlock;
			}
		}

		private bool disposed = false;

		#region Dispose pattern
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
				if (disposing)
				{
					stream.Close();
					stream = null;
					reader = null;
					writer = null;
					streamLock = null;
				}
			disposed = true;
		}
		#endregion
	}
}
