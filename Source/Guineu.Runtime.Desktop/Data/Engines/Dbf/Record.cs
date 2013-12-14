using System;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Diagnostics;
using Guineu.Expression;

namespace Guineu.Data.Dbf
{
	class Record
	{
		readonly Int64 recordNumber;
		readonly FieldDefinition definition;
		readonly MemoFile memo;
		readonly IField[] fields;
		readonly Boolean[] changed;
		readonly IFieldFactory factory;


		/// <summary>
		/// Creates an empty record.
		/// </summary>
		public Record(Int64 recNo, FieldDefinition def, MemoFile memo, Encoding enc, Byte[] content, IFieldFactory factory)
		{
			this.factory = factory;
			definition = def;
			this.memo = memo;
			recordNumber = recNo;
			fields = new IField[definition.Count + 1];
			changed = new Boolean[definition.Count + 1];

			CreateFieldValues(definition, enc);
			if (recNo != 0)
				LoadRecord(definition, content, this.memo);
		}

		void CreateFieldValues(FieldDefinition fd, Encoding enc)
		{
			fields[0] = new DeletedField();
			for (Int32 f = 1; f <= fd.Count; f++)
				fields[f] = factory.Create(fd[f], enc);
		}

		void LoadRecord(FieldDefinition fd, Byte[] content, MemoFile memoFile)
		{
			Stream stm = new MemoryStream(content);

			using (var reader = new BinaryReader(stm))
			{
				fields[0].Read(reader);
				for (Int32 f = 1; f <= fd.Count; f++)
				{
					fields[f].VariableLength = fd[f].BitVarField > 0 && ReadBit(content, fd[f].BitVarField);
					stm.Seek(fd[f].Position, SeekOrigin.Begin);
					fields[f].Read(reader);
					fields[f].ReadMemo(memoFile);
				}
			}
		}

		public void Delete()
		{
			changed[0] = true;
			fields[0].Value = new Variant(true);
		}

		public Boolean Deleted()
		{
			return fields[0].Value;
		}

		public Int64 RecordLoaded
		{
			get { return recordNumber; }
		}

		public void Gather(Byte[] content)
		{
			using (var writer = new BinaryWriter(new MemoryStream(content)))
			{
				fields[0].Write(writer);
				for (Int32 nFld = 1; nFld < changed.Length; nFld++)
				{
					if (recordNumber == 0 || changed[nFld])
					{
						var fld = definition[nFld];
						writer.BaseStream.Seek(fld.Position, SeekOrigin.Begin);
						WriteField(content, fields[nFld], fld, writer, memo);
					}
					if (GuineuInstance.DebugLogRecordGather)
						using (var sw = new StreamWriter("record.gather.txt", true))
							sw.WriteLine(DateTime.Now + ", change " + definition[nFld].Name);
				}
			}
		}

		public Variant AsVariant(Int32 fld)
		{
			return fields[fld + 1].Value;
		}

		public void SetField(Nti fieldName, Variant val)
		{
			var fld = definition[fieldName];
			if (val.IsNull && fld.BitNullable == 0)
				throw new ErrorException(ErrorCodes.FieldDoesNotAcceptNullValue, fld.Name);
			changed[fld.FieldNo] = true;
			fields[fld.FieldNo].Value = val;
		}

		void WriteField(Byte[] raw, IField f, FieldDefinitionEntry fld, BinaryWriter sw, MemoFile memoFile)
		{
			if (f.Value.IsNull)
				if (fld.BitNullable == 0)
					throw new ErrorException(ErrorCodes.FieldDoesNotAcceptNullValue, fld.Name);
				else
					WriteBit(raw, fld.BitNullable, true);
			else
			{
				f.WriteMemo(memoFile);
				f.Write(sw);
				if (fld.BitVarField > 0)
					WriteBit(raw, fld.BitVarField, f.VariableLength);
				if (fld.BitNullable > 0)
					WriteBit(raw, fld.BitNullable, false);
			}
		}

		Boolean ReadBit(Byte[] raw, short bit)
		{
			return Flags(raw)[bit];
		}

		BitArray Flags(Byte[] raw)
		{
			using (var r = new BinaryReader(new MemoryStream(raw)))
			{
				FieldDefinitionEntry nullFlags = definition.NullFlags;
				r.BaseStream.Seek(nullFlags.Position, SeekOrigin.Begin);
				var flags = new BitArray(r.ReadBytes(nullFlags.Length));
				return flags;
			}
		}

		void WriteBit(Byte[] raw, short bit, Boolean value)
		{
			var flags = Flags(raw);
			flags[bit] = value;
			FieldDefinitionEntry nullFlags = definition.NullFlags;
			var ba = new Byte[nullFlags.Length];
			flags.CopyTo(ba, 0);
			using (var writer = new BinaryWriter(new MemoryStream(raw)))
			{
				writer.BaseStream.Seek(nullFlags.Position, SeekOrigin.Begin);
				writer.Write(ba);
			}
		}
	}

	interface IFieldFactory
	{
		IField Create(FieldDefinitionEntry def, Encoding enc);
	}
	class FieldFactory : IFieldFactory
	{
		public IField Create(FieldDefinitionEntry fld, Encoding enc)
		{
			switch (fld.FType)
			{
				case FieldTypes.Integer:
					return new IntegerField();
				case FieldTypes.Blob:
					return new MemoField(Encoding.Default);
				case FieldTypes.Character:
					return new CharacterField(fld.Length, enc, true);
				case FieldTypes.Currency:
					return new CurrencyField();
				case FieldTypes.Date:
					return new DateField();
				case FieldTypes.DateTime:
					return new DateTimeField();
				case FieldTypes.Double:
					return new DoubleField(fld.Precision);
				case FieldTypes.General:
					return new MemoField(Encoding.Default);
				case FieldTypes.Logical:
					return new LogicalField();
				case FieldTypes.Memo:
					return new MemoField(enc);
				case FieldTypes.Float:
				case FieldTypes.Numeric:
					return new NumberField(fld.Length, fld.Precision);
				case FieldTypes.Picture:
					return new MemoField(Encoding.Default);
				case FieldTypes.Varbinary:
					return new CharacterField(fld.Length, Encoding.Default, false);
				case FieldTypes.Varchar:
					return new CharacterField(fld.Length, enc, false);
				default:
					return new CharacterField(fld.Length, enc, true);
			}
		}
	}


	interface IField
	{
		void Write(BinaryWriter writer);
		void WriteMemo(MemoFile memo);
		void Read(BinaryReader reader);
		void ReadMemo(MemoFile memo);
		Boolean VariableLength { get; set; }
		Variant Value { get; set; }
	}

	class CharacterField : IField
	{
		readonly Int32 maxLength;
		readonly Encoding encoding;
		readonly Boolean padding;
		Int32 originalLength;

		public CharacterField(Int32 len, Encoding enc, Boolean pad)
		{
			maxLength = len;
			encoding = enc;
			padding = pad;
			Value = new Variant("");
		}

		public void Write(BinaryWriter writer)
		{
			Byte[] buf = encoding.GetBytes(Value);
			if (VariableLength)
			{
				Debug.Assert(!padding);
				buf[maxLength - 1] = (Byte)originalLength;
			}
			writer.Write(buf, 0, maxLength);
		}

		public void Read(BinaryReader reader)
		{
			Byte[] buf = reader.ReadBytes(maxLength);
			Int32 length = VariableLength ? buf[maxLength - 1] : maxLength;
			Value = new Variant(encoding.GetString(buf, 0, length));
		}

		private Variant variantValue;
		public Variant Value
		{
			get { return variantValue; }
			set
			{
				var s = (String) value;
				if (s == null)
					variantValue = new Variant(VariantType.Character, true);
				else
				{
					if (s.Length > maxLength)
						s = s.Substring(0, maxLength);
					originalLength = s.Length;
					if (padding)
						s = s.PadRight(maxLength);
					VariableLength = (s.Length != maxLength);
					variantValue = new Variant(s);
				}
			}
		}

		public void WriteMemo(MemoFile memo) { }
		public void ReadMemo(MemoFile memo) { }
		public bool VariableLength { get; set; }
	}

	class DateTimeField : IField
	{
		public DateTimeField()
		{
			Value = new Variant(new DateTime(0));
		}

		public void Write(BinaryWriter writer)
		{
			DateTime value = Value;

			int y = value.Year;
			int m = value.Month;
			int day = value.Day;
			const int igreg = 15 + 31 * (10 + 12 * 1582);

			if (y < 0) y = y + 1;
			if (m > 2)
				m = m + 1;
			else
			{
				y = y - 1;
				m = m + 13;
			}

			int ijulian = (int)(365.25 * y) + (int)(30.6001 * m) + day + 1720995;

			if (day + 31 * (m + 12 * y) >= igreg)
			{
				// change for Gregorian calendar
				int adj = y / 100;
				ijulian = ijulian + 2 - adj + adj / 4;
			}

			
			Int32 dayPart = ijulian;
			Int32 millisecondPart = 1000 * value.Second + value.Minute * 1000 * 60 + value.Hour * 1000 * 60 * 60;

			// TODO: Add a way to handle empty values
			if (value.Year == 1 && value.Month == 1 && value.Day == 1 && millisecondPart == 0)
				dayPart = 0;
			writer.Write(dayPart);
			writer.Write(millisecondPart);
		}

		public void Read(BinaryReader reader)
		{
			Int32 julianDate = reader.ReadInt32();
			Int32 milliSeconds = reader.ReadInt32();

			if (julianDate == 0 && milliSeconds == 0)
			{
				Value = new Variant(new DateTime(0));
				return;
			}
			// (...) temporary until raw buffer is correct based on type
			if (julianDate == 0x20202020 && milliSeconds == 0x20202020)
			{
				Value = new Variant(new DateTime(0));
				return;
			}

			double jd = julianDate;

			double z = Math.Floor(jd + 0.5);
			double w = Math.Floor((z - 1867216.25) / 36524.25);
			double x = Math.Floor(w / 4);
			double aa = Math.Floor(z + 1 + w - x);
			double bb = Math.Floor(aa + 1524);
			double cc = Math.Floor((bb - 122.1) / 365.25);
			double dd = Math.Floor(365.25 * cc);
			double ee = Math.Floor((bb - dd) / 30.6001);
			double ff = Math.Floor(30.6001 * ee);

			double day = bb - dd - ff;
			double month;
			double year;

			if ((ee - 13) <= 12 && (ee - 13) > 0)
				month = ee - 13;
			else
				month = ee - 1;

			if (month == 1 || month == 2)
				year = cc - 4715;
			else
				year = cc - 4716;
			var dt = new DateTime((Int32)year, (Int32)month, (Int32)day);

			// Visual FoxPro stores DateTime values with a precision of milliseconds, but 
			// often doesn't use a rounded value. In the table we find 3599900 ms when the
			// time portion is 01:00:00. Visual FoxPro rounds them to the nearest second 
			// when reading the field. 
			//
			// Without the conversion into double and the rounding, the result in Guineu
			//would frequently be one second less than in Visual FoxPro.
			dt = dt.AddSeconds((Int32)Math.Round((Double)milliSeconds / 1000));
			Value = new Variant(dt);
		}

		public Variant Value { get; set; }
		public void WriteMemo(MemoFile memo) { }
		public void ReadMemo(MemoFile memo) { }
		public bool VariableLength { get; set; }
	}
	class CurrencyField : IField
	{
		public void Write(BinaryWriter writer)
		{
			Double value = Value;
			var convertedValue = (Int32)(value * 10000);
			writer.Write(convertedValue);
		}

		public void Read(BinaryReader reader)
		{
			Int32 retVal = reader.ReadInt32();
			Value = new Variant(((Double)retVal) / 10000, 21, 4);
		}

		public Variant Value { get; set; }
		public void WriteMemo(MemoFile memo) { }
		public void ReadMemo(MemoFile memo) { }
		public bool VariableLength { get; set; }
	}
	class IntegerField : IField
	{
		public IntegerField()
		{
			Value = new Variant(0, 10);
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write((Int32) Value);
		}

		public void Read(BinaryReader reader)
		{
			Int32 i = reader.ReadInt32();
			Value = new Variant(i, 10);
		}

		public Variant Value { get; set; }
		public void WriteMemo(MemoFile memo) { }
		public void ReadMemo(MemoFile memo) { }
		public bool VariableLength { get; set; }
	}
	class NumberField : IField
	{
		readonly Int32 length;
		readonly Int32 precision;
		readonly static NumberFormatInfo NumericFormat;

		static NumberField()
		{
			var ci = CultureInfo.CurrentUICulture;
			NumericFormat = (NumberFormatInfo)ci.NumberFormat.Clone();
			NumericFormat.NumberDecimalSeparator = ".";
		}

		public NumberField(Int32 len, Int32 precision)
		{
			length = len;
			this.precision = precision;
			Value = new Variant(0, len, precision);
		}

		public void Write(BinaryWriter writer)
		{
			String format;
			if (precision > 0)
				format = "{0," + length + ":#." + new string('0', precision) + "}";
			else
				format = "{0," + length + ":#}";
			var numericValue = String.Format(NumericFormat, format, (Double) Value);
			Byte[] buf = Encoding.ASCII.GetBytes(numericValue);
			writer.Write(buf, 0, length);
		}

		public void Read(BinaryReader reader)
		{
			Byte[] buf = reader.ReadBytes(length);
			String strNum = Encoding.ASCII.GetString(buf, 0, length);
			if (strNum.TrimEnd().Length == 0)
				Value = new Variant(0, 11);
			else
				try
				{
					Double dblNum = Double.Parse(strNum, NumericFormat);
					Value = new Variant(dblNum, length, precision);
				}
				catch (FormatException)
				{
					// TODO: Find a number for ********
					Value = new Variant(0, 11);
				}
		}

		public Variant Value { get; set; }
		public void WriteMemo(MemoFile memo) { }
		public void ReadMemo(MemoFile memo) { }
		public bool VariableLength { get; set; }
	}
	class DateField : IField
	{
		const Int32 Length = 8;

		public DateField()
		{
			Value = new Variant(new DateTime(0), VariantType.Date);
		}

		public void Write(BinaryWriter writer)
		{
			String dateValue = ((DateTime) Value).ToString("yyyyMMdd");
			Byte[] buf = Encoding.ASCII.GetBytes(dateValue);
			writer.Write(buf, 0, Length);
		}

		public void Read(BinaryReader reader)
		{
			Byte[] buf = reader.ReadBytes(Length);
			String strDate = Encoding.ASCII.GetString(buf, 0, Length);
			DateTime dateValue;
			if (strDate == "        ")
				dateValue = new DateTime(0);
			else if (strDate == "00010101")
				dateValue = new DateTime(1);
			else
				dateValue = DateTime.ParseExact(strDate, "yyyyMMdd", CultureInfo.CurrentCulture);
			Value = new Variant(dateValue, VariantType.Date);

		}

		public Variant Value { get; set; }
		public void WriteMemo(MemoFile memo) { }
		public void ReadMemo(MemoFile memo) { }
		public bool VariableLength { get; set; }
	}
	class LogicalField : IField
	{
		const Byte True = 84;    // "T"
		const Byte TrueYes = 89; // "Y"
		const Byte False = 70;   // "F"

		public LogicalField()
		{
			Value = new Variant(false);
		}

		public void Write(BinaryWriter writer)
		{
			if (Value)
				writer.Write(True);
			else
				writer.Write(False);
		}

		public void Read(BinaryReader reader)
		{
			Byte b = reader.ReadByte();
			if ((b == True) || (b == TrueYes))
				Value = new Variant(true);
			else
				Value = new Variant(false);
		}

		public Variant Value { get; set; }
		public void WriteMemo(MemoFile memo) { }
		public void ReadMemo(MemoFile memo) { }
		public bool VariableLength { get; set; }
	}
	class DoubleField : IField
	{
		const Int32 Length = 21;
		readonly Int32 precision;

		public DoubleField(Int32 precision)
		{
			this.precision = precision;
			Value = new Variant(0, Length, this.precision);
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write((Double) Value);
		}

		public void Read(BinaryReader reader)
		{
			Value = new Variant(reader.ReadDouble(), 21, precision);
		}

		public Variant Value { get; set; }
		public void WriteMemo(MemoFile memo) { }
		public void ReadMemo(MemoFile memo) { }
		public bool VariableLength { get; set; }
	}
	class MemoField : IField
	{
		readonly Encoding encoding;
		Int32 block;

		public MemoField(Encoding enc)
		{
			encoding = enc;
			Value = new Variant("");
		}

		public void WriteMemo(MemoFile memo)
		{
			Byte[] data = GuineuInstance.CurrentCp.GetBytes(Value);
			block = memo.Write(data, block);
		}

		public void Write(BinaryWriter writer)
		{
			// TODO: Check the table version to differentiate 10 bytes vs 4 byts
			writer.Write(block);
		}

		public void ReadMemo(MemoFile memo)
		{
			Byte[] bufMemo;
			// (...) find a better way to detect empty records...
			// (...) empty memo fields are always 0x00000000
			if (block == 0x20202020 || block == 0x00000000)
				bufMemo = new Byte[0];
			else
				bufMemo = memo.Read(block);
			Value = new Variant(encoding.GetString(bufMemo, 0, bufMemo.Length));
		}
		public void Read(BinaryReader reader)
		{
			block = reader.ReadInt32();
		}

		public Variant Value { get; set; }
		public bool VariableLength { get; set; }
	}
	class DeletedField : IField
	{
		const Byte True = 0x2A;
		const Byte False = 0x20;

		public DeletedField()
		{
			Value = new Variant(false);
		}

		public void Write(BinaryWriter writer)
		{
			if (Value)
				writer.Write(True);
			else
				writer.Write(False);
		}

		public void Read(BinaryReader reader)
		{
			Byte b = reader.ReadByte();
			Value = b == True ? new Variant(true) : new Variant(false);
		}

		public Variant Value { get; set; }
		public void WriteMemo(MemoFile memo) { }
		public void ReadMemo(MemoFile memo) { }
		public bool VariableLength { get; set; }
	}
}

// TODO: Handle NOCPTRANS