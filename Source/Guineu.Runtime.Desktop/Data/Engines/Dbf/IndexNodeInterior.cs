using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Data.Dbf
{
	class IndexNodeInterior : IndexNode
	{
		IndexEntry[] _Keys;

		public IndexNodeInterior(Byte[] buffer, Int16 keyLength)
			: base(buffer,keyLength)
		{
			ReadKeys();
		}

		private void ReadKeys()
		{
			_Keys = new IndexEntry[KeyCount];
			for (Int16 CurKey = 0; CurKey < KeyCount; CurKey++)
			{
				_Keys[CurKey].Value = Reader.ReadBytes(KeyLength);
				_Keys[CurKey].RecNo =
					Reader.ReadByte() * 256 * 256 * 256 +
					Reader.ReadByte() * 256 * 256 +
					Reader.ReadByte() * 256 +
					Reader.ReadByte();
				_Keys[CurKey].Node = Reader.ReadByte() * 256 * 256 * 256 +
					Reader.ReadByte() * 256 * 256 +
					Reader.ReadByte() * 256 +
					Reader.ReadByte();
			}
		}

		internal Int32 Locate(byte[] value, bool exact)
		{
			for (Int16 CurKey = 0; CurKey < _Keys.Length; CurKey++)
			{
				if (KeyFound(value, exact, CurKey))
					return _Keys[CurKey].Node;
			}
			return 0;
		}

		private bool KeyFound(byte[] value, bool exact, Int16 CurKey)
		{
			return Util.BufferUtil.Compare(_Keys[CurKey].Value, value, exact) >= 0;
		}

		internal Int32 Locate(byte[] value, Int32 recNo)
		{
			for (Int16 CurKey = 0; CurKey < _Keys.Length; CurKey++)
			{
				if (KeyFound(value, true, CurKey) && (_Keys[CurKey].RecNo >= recNo))
					return _Keys[CurKey].Node;
			}
			return 0;
		}

		internal int GetFirstNode()
		{
			if (_Keys.Length > 0)
				return _Keys[0].Node;
			else
				return 0;
		}

		internal int GetLastNode()
		{
			if (_Keys.Length > 0)
				return _Keys[_Keys.Length-1].Node;
			else
				return 0;
		}
	}
}
