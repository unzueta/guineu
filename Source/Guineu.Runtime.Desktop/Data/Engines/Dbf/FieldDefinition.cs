using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;
using Guineu.Util;

namespace Guineu.Data.Dbf
{
	class FieldDefinition
	{
		readonly List<FieldDefinitionEntry> fields;
		readonly FieldDefinitionEntry nullFlags;

		internal FieldDefinition(Int32 cnt, BinaryReader r, Encoding decoder)
		{
			fields = new List<FieldDefinitionEntry>(cnt - 1);

			Int16 bitPosition = 0;
			for (Int32 curField = 0; curField < cnt; curField++)
			{
				var fld = new FieldDefinitionEntry();
				Byte[] name = r.ReadBytes(11);
				Int32 length = 0;
				for (Int32 i = 0; i < name.Length; i++)
				{
					if (name[i] == 0)
					{
						length = i;
						break;
					}
				}
				fld.FieldNo = curField + 1;
				fld.Name = decoder.GetString(name, 0, length);
				fld.Nti = new Nti(fld.Name);
				fld.FType = (FieldTypes)r.ReadByte();
				fld.Position = r.ReadUInt32();
				fld.Length = r.ReadByte();
				fld.Precision = r.ReadByte();
				fld.Flags = (FieldFlags)r.ReadByte();
				fld.NextAutoInc = r.ReadUInt32();
				fld.Increment = r.ReadByte();
				r.ReadBytes(8);

				// count the number of regular fields in the table
				if (EnumUtil.IsSet((Int32)fld.Flags, (Int32)FieldFlags.SystemColumn))
				{
					if (String.Compare("_NullFlags", fld.Name, true) == 0)
						nullFlags = fld;
				}
				else
					fields.Add(fld);

				// Determine the number of bits in the _NullFlags field
				if (EnumUtil.IsSet((Int32)fld.Flags, (Int32)FieldFlags.Nullable))
				{
					fld.BitNullable = bitPosition;
					bitPosition++;
				}
				if (fld.VarField)
				{
					fld.BitVarField = bitPosition;
					bitPosition++;
				}
			}
		}

		/// <summary>
		/// Returns the definition of a particular field
		/// </summary>
		/// <param name="fieldNo"></param>
		/// <returns></returns>
		public FieldDefinitionEntry this[Int32 fieldNo]
		{
			get { return fields[fieldNo - 1]; }
		}

		public FieldDefinitionEntry this[Nti field]
		{
			get
			{
				for (Int32 fieldNo = 0; fieldNo < fields.Count; fieldNo++)
					if (field == fields[fieldNo].Nti)
						return fields[fieldNo];

				throw new ErrorException(ErrorCodes.VariableNotFound,field);
			}
		}

		public Int32 Count
		{
			get { return fields.Count; }
		}

		internal FieldDefinitionEntry NullFlags
		{
			get { return nullFlags; }
		}
	}

	enum FieldTypes : byte
	{
		Blob = (byte)'W',
		Character = (byte)'C',
		Currency = (byte)'Y',
		Double = (byte)'B',
		Date = (byte)'D',
		DateTime = (byte)'T',
		Float = (byte)'F',
		General = (byte)'G',
		Integer = (byte)'I',
		Logical = (byte)'L',
		Memo = (byte)'M',
		Numeric = (byte)'N',
		Picture = (byte)'P',
		Varbinary = (byte)'Q',
		Varchar = (byte)'V'
	}

	static class FieldTypesExtensions
{
	public static ColumnType ToColumnType(this FieldTypes obj)
	{
		switch (obj)
		{
			case FieldTypes.Blob:
				return ColumnType.Blob;
			case FieldTypes.Character:
				return ColumnType.Character;
			case FieldTypes.Currency:
				return ColumnType.Currency;
			case FieldTypes.Double:
				return ColumnType.Double;
			case FieldTypes.Date:
				return ColumnType.Date;
			case FieldTypes.DateTime:
				return ColumnType.DateTime;
			case FieldTypes.Float:
				return ColumnType.Float;
			case FieldTypes.General:
				return ColumnType.General;
			case FieldTypes.Integer:
				return ColumnType.Integer;
			case FieldTypes.Logical:
				return ColumnType.Logical;
			case FieldTypes.Memo:
				return ColumnType.Memo;
			case FieldTypes.Numeric:
				return ColumnType.Numeric;
			case FieldTypes.Picture:
				return ColumnType.Picture;
			case FieldTypes.Varbinary:
				return ColumnType.Varbinary;
			case FieldTypes.Varchar:
				return ColumnType.Varchar;
			default:
				throw new ArgumentOutOfRangeException("obj");
		}
	}
}

	[Flags]
	enum FieldFlags : byte
	{
		SystemColumn = 0x01,
		Nullable = 0x02,       // NULL option on CREATE TABLE statement
		Binary = 0x04,         // Corresponds to NOCPTRANS
		AutoIncrement = 0x08   // always combined with FieldFlags.Binary.
	}

	class FieldDefinitionEntry
	{
		public Int32 FieldNo;
		public String Name;
		public Nti Nti;
		public FieldTypes FType;
		public Int32 Length;
		public Int32 Precision;
		public Int64 Position;
		public FieldFlags Flags;
		public UInt32 NextAutoInc;
		public Byte Increment;

		/// <summary>
		/// Is true when the field contains variable length data and is not a memo field.
		/// </summary>
		public Boolean VarField
		{
			get
			{
				if (FType == FieldTypes.Varbinary || FType == FieldTypes.Varchar)
					return true;
				return false;
			}
		}

		public Int16 BitNullable = -1;
		public Int16 BitVarField = -1;
	}

}
