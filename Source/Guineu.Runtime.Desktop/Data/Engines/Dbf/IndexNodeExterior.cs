using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Core;
using Guineu.Util;

namespace Guineu.Data.Dbf
{
	sealed public class IndexNodeExterior : IndexNode
	{

		Int16 _FreeSpace;

		Int32 _RecordNumberMask;
		Byte _DuplicateCountMask;
		Byte _TrailingByteMask;

		Byte _BitsRecNo;
		/// <summary>
		/// The duplicate count specifies how many characters are repeated from 
		/// the previous value.
		/// </summary>
		Byte _BitsDuplicate;
		/// <summary>
		/// The trailing count specifies the number of blanks that are added to
		/// the end of the key value.
		/// </summary>
		Byte _BitsTrailing;
		Byte _Size;

		IndexEntry[] _Keys;

		public IndexNodeExterior(Byte[] buffer, Int16 keyLength)
			: base(buffer, keyLength)
		{
			_FreeSpace = Reader.ReadInt16();
			_RecordNumberMask = Reader.ReadInt32();
			_DuplicateCountMask = Reader.ReadByte();
			_TrailingByteMask = Reader.ReadByte();
			_BitsRecNo = Reader.ReadByte();
			_BitsDuplicate = Reader.ReadByte();
			_BitsTrailing = Reader.ReadByte();
			_Size = Reader.ReadByte();
			ReadKeys();
		}

		///// <summary>
		///// Returns a buffer 
		///// </summary>
		///// <returns></returns>
		//public Byte[] GetBuffer()
		//{
		//}

		private void ReadKeys()
		{
			Int32 ValuePosition = 0;
			Stream BS = Reader.BaseStream;
			CompactEntry Entry;
			Int32 DataLen;

			_Keys = new IndexEntry[KeyCount];
			for (Int16 CurKey = 0; CurKey < KeyCount; CurKey++)
			{
				// Read key entry
				BS.Seek(24 + CurKey * _Size, SeekOrigin.Begin);
				Byte[] KeyCompact = Reader.ReadBytes(_Size);
				Entry = ParseEntry(KeyCompact);
				_Keys[CurKey].RecNo = Entry.RecNo;

				// Read index value
				DataLen = KeyLength - Entry.Duplicate - Entry.Trailing;
				ValuePosition += DataLen;
				BS.Seek(-1 * ValuePosition, SeekOrigin.End);
				Byte[] data;
				if (Entry.Duplicate > 0)
				{
					data = new Byte[KeyLength - Entry.Trailing];
					Array.Copy(_Keys[CurKey - 1].Value, data, Entry.Duplicate);
					Array.Copy(Reader.ReadBytes(DataLen), 0, data, Entry.Duplicate, DataLen);
				}
				else
				{
					data = Reader.ReadBytes(DataLen);
				}
				_Keys[CurKey].Value = new Byte[KeyLength];
				Array.Copy(data, _Keys[CurKey].Value, data.Length);
			}
		}

		private CompactEntry ParseEntry(byte[] KeyCompact)
		{
			CompactEntry ret;
			if (_Size <= 4)
			{
				//Int32 KeyEntry = BitConverter.ToInt32(KeyCompact, 0);
				Int32 KeyEntry = 0;
				for (Int32 i = KeyCompact.Length - 1; i >= 0; i--)
					KeyEntry = KeyEntry * 256 + KeyCompact[i];
				ret.RecNo = (Int32)(KeyEntry & _RecordNumberMask);
				KeyEntry = KeyEntry >> _BitsRecNo;
				ret.Duplicate = (Byte)(KeyEntry & _DuplicateCountMask);
				KeyEntry = KeyEntry >> _BitsDuplicate;
				ret.Trailing = (Byte)(KeyEntry & _TrailingByteMask);
			}
			else
			{
				Int64 KeyEntry = BitConverter.ToInt64(KeyCompact, 0);
				ret.RecNo = (Int32)(KeyEntry & _RecordNumberMask);
				KeyEntry = KeyEntry >> _BitsRecNo;
				ret.Duplicate = (Byte)(KeyEntry & _DuplicateCountMask);
				KeyEntry = KeyEntry >> _BitsDuplicate;
				ret.Trailing = (Byte)(KeyEntry & _TrailingByteMask);
			}
			return ret;
		}

		public SeekResult Locate(Byte[] search, Int32 start, Boolean exact)
		{
			SeekResult result = new SeekResult(false,0);
			Int32 comp;
			for (Int32 Entry = start; Entry < _Keys.Length; Entry++)
			{
				result.Record = _Keys[Entry].RecNo;
				result.Index = Entry;
				comp = Util.BufferUtil.Compare(_Keys[Entry].Value, search, exact);
				if (comp == 0)
					result.Found = true;
				if (comp >= 0)
					break;
			}
			return result;
		}

		private struct CompactEntry
		{
			public Int32 RecNo;
			public Byte Duplicate;
			public Byte Trailing;
		}

		internal long GetFirstRecord()
		{
			if (_Keys.Length > 0)
				return _Keys[0].RecNo;
			else
				return 0;
		}

		internal long GetLastRecord()
		{
			if (_Keys.Length > 0)
				return _Keys[_Keys.Length - 1].RecNo;
			else
				return 0;
		}

		internal int GetLastKey()
		{
			return _Keys.Length;
		}

		internal Int32 GetPreviousNode()
		{
			return LeftNode;
		}

		internal long GetRecordNumber(int key)
		{
			return _Keys[key].RecNo;
		}

		internal Int32 GetKeyCount()
		{
			return _Keys.Length;
		}

		internal Int32 GetNextNode()
		{
			return RightNode;
		}

		public Byte[] GetKey(Int32 index)
		{
			return _Keys[index].Value;
		}
		internal void RemoveKey(Int32 index)
		{
			Int32 src;
			IndexEntry[] newKeys = new IndexEntry[_Keys.Length - 1];
			for (src = 0; src < index; src++)
				newKeys[src] = _Keys[src];
			for (src = index + 1; src < _Keys.Length; src++)
				newKeys[src - 1] = _Keys[src];
			_Keys = newKeys;
		}
	}

	struct IndexEntry
	{
		internal Byte[] Value;
		internal Int32 RecNo;
		internal Int32 Node;
	}

}
