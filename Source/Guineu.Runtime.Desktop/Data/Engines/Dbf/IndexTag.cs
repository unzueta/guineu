using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Data.Engines.Dbf.Index;
using Guineu.Util;
using Guineu.Expression;
using Guineu.Data.Dbf.Index;

namespace Guineu.Data.Dbf
{
	public class IndexTag
	{
		IndexHeader _Header;
		IndexNodeReader nodeReader;
		KeyItem keyFactory;
		KeyList keys;

		public IndexTag(IndexNodeReader nodeReader, Int32 Header)
		{
			_Header = nodeReader.GetHeader(Header);
			this.nodeReader = nodeReader;
			IndexReaderWriter rw = new IndexReaderWriter(nodeReader, _Header);
			keys = new KeyList(rw);
			// (...) Code page should be the one from the table rather then the default CP.
			keyFactory = new KeyItem(
				  Enum<IndexOptions>.IsSet(_Header._Options, IndexOptions.Null)
				, GuineuInstance.CurrentCp
				, _Header.KeyLength
			);
		}

		public KeyItem KeyItem
		{
			get { return keyFactory; }
		}

		public IndexHeader Header
		{
			get { return _Header; }
		}
		public IndexNodeReader NodeReader
		{
			get { return nodeReader; }
		}

		public void UpdateCurrentRecord(Int64 recNo)
		{
			Variant value = this.EvaluateExpression();
			KeyItem item = keyFactory.New(value,recNo);
			// TODO: IndexFile needs to delete the old key and add the new key.
			// TODO: Table needs to keep track of what the current record is. Must be the old
			// content when removing and the new one when adding the key.
		}
		public SeekResult Seek(KeyItem key, SeekOptions options)
		{
			Boolean exact = Enum<SeekOptions>.IsSet(options, SeekOptions.Exact);
			KeyItem found = keys.Find(key);
			SeekResult result = new SeekResult(found.Matches(key, exact), found.GetRecord());
			return result;
		}

		/// <summary>
		/// Locates the record that is the closest match for the current record
		/// in the table.
		/// </summary>
		public void LocateCurrentRecord()
		{
			//Variant value = this.EvaluateExpression();
			//Byte[] data = this.VariantToByte(value);
			//Byte[] data2 = new Byte[data.Length + 4];
			//BinaryWriter bw = new BinaryWriter(new MemoryStream(data2));
			//bw.Write(data);
			//bw.Write(1);
			
			//this.Seek(data, true);
		}

		Variant EvaluateExpression()
		{
			Variant value = this._Header.Get(GuineuInstance.Context.CurrentContext);
			return value;
		}
	}

	public struct SeekResult
	{
		public SeekResult(Boolean found, Int64 record)
		{
			Found = found;
			Record = record;
			Index = -1;
		}
		internal Boolean Found;
		internal Int64 Record;
		internal Int32 Index;
	}

	[Flags]
	public enum SeekOptions
	{
		None = 0,
		Exact = 1,
		Near = 2
	}
}
