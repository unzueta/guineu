using System;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class RESTORE : ICommand
	{
		ExpressionBase File;
		ExpressionBase Memo;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);

			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.FROM:
						code.Reader.ReadToken();
						nextToken = code.Reader.PeekToken();
						if (nextToken != Token.MEMO)
							File = Comp.GetCompiledExpression();
						break;
					case Token.MEMO:
						code.Reader.ReadToken();
						Memo = Comp.GetCompiledExpression();
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext ctx, ref Int32 nextLine)
		{
			if (File != null)
			{
				String name = File.GetString(ctx);
				using (Stream fs = GuineuInstance.FileMgr.Open(name, FileMode.Open))
					ReadMemFile(ctx, fs);
			}
			else
			{
				String memo = Memo.GetString(ctx);
				using (Stream fs = new MemoryStream(GuineuInstance.CurrentCp.GetBytes(memo)))
				{
					fs.Position = 2;
					ReadMemFile(ctx, fs);
				}
			}

		}

		void ReadMemFile(CallingContext ctx, Stream s)
		{
			using (BinaryReader br = new BinaryReader(s))
				while (s.Position < s.Length && br.PeekChar() != 0x1A)
				{
					NamedValue val = ReadVariant(br);
					MemberList locals = ctx.Locals;
					if (val.D1 == 0)
						locals.Set(val.Name, val.Value);
					else
					{
						ArrayMember arr;
						if (val.D2 == 0)
							arr = new ArrayMember(val.D1);
						else
							arr = new ArrayMember(val.D1, val.D2);
						locals.Add(val.Name, arr);
						if (val.D2 == 0)
							for (Int32 u = 1; u <= val.D1; u++)
								ReadElement(br, arr, 1, u);
						else
							for (Int32 i = 1; i <= val.D1; i++)
								for (Int32 u = 1; u <= val.D2; u++)
									ReadElement(br, arr, i, u);
					}
				}
		}

		static void ReadElement(BinaryReader br, ArrayMember arr, int d1, int d2)
		{
			NamedValue val = ReadVariant(br);
			ValueMember vm = arr.Locate(d1, d2);
			vm.Set(val.Value);
		}


		struct NamedValue
		{
			public Variant Value;
			public Nti Name;
			public Int32 D1;
			public Int32 D2;
		}

		static NamedValue ReadVariant(BinaryReader br)
		{
			NamedValue retVal;
			var name = new String(br.ReadChars(11));
			name = name.Substring(0, name.IndexOf('\0'));
			retVal.Value = new Variant();
			retVal.D1 = 0;
			retVal.D2 = 0;
			var type = (MemFileType)br.ReadByte();
			var lenLong = br.ReadInt32();
			Byte len = br.ReadByte();
			Byte dec = br.ReadByte();
			br.ReadBytes(14);

			if (String.IsNullOrEmpty(name))
			{
				Int16 len2 = br.ReadInt16();
				name = new String(br.ReadChars(len2));
			}

			retVal.Name = new Nti(name);
			switch (type)
			{
				case MemFileType.NumericLong:
				case MemFileType.Numeric:
					Double value = br.ReadDouble();
					retVal.Value = new Variant(value, 18, dec);
					break;

				case MemFileType.Character:
				case MemFileType.CharacterLong:
					Byte[] byte1 = br.ReadBytes(len - 1);
					String strValue = new String(GuineuInstance.CurrentCp.GetChars(byte1));
					br.ReadByte();
					retVal.Value = new Variant(strValue);
					break;

				case MemFileType.Memo:
				case MemFileType.MemoLong:
					Byte[] byte2 = br.ReadBytes(lenLong - 1);
					String strValue2 = new String(GuineuInstance.CurrentCp.GetChars(byte2));
					br.ReadByte();
					retVal.Value = new Variant(strValue2);
					break;

				case MemFileType.DateTime:
				case MemFileType.Date:
				case MemFileType.DateTimeLong:
				case MemFileType.DateLong:
					Int32 i1 = br.ReadInt32();
					Int32 i2 = br.ReadInt32();
					Int32 sec = (i1 & 0x7FFFFFFF) / (0x7FFFFFFF / 86400);
					Int32 days = (i2 << 1) - (i1 >> 31);
					days = days & 0x7FFFFFFF;
					DateTime dt = new DateTime(2004, 5, 22);
					days = days - 0x2856E9C;
					dt = dt.AddDays(days);
					dt = dt.AddSeconds(sec);
					if (type == MemFileType.Date || type == MemFileType.DateLong)
						retVal.Value = new Variant(dt, VariantType.Date);
					else
						retVal.Value = new Variant(dt);
					break;

				case MemFileType.Array:
				case MemFileType.ArrayLong:
					retVal.D1 = br.ReadUInt16();
					retVal.D2 = br.ReadUInt16();
					break;

				case MemFileType.Logical:
				case MemFileType.LogicalLong:
					Boolean bValue = br.ReadBoolean();
					retVal.Value = new Variant(bValue);
					break;

				default:
					break;
			}

			return retVal;

		}
	}

	// ReSharper disable CharImplicitlyConvertedToNumeric
	enum MemFileType
	{
		Numeric = 'N',
		Character = 'C',
		DateTime = 'T',
		Date = 'D',
		Array = 'A',
		Logical = 'L',
		Memo = 'H',

		NumericLong = 'n',
		CharacterLong = 'c',
		DateTimeLong = 't',
		DateLong = 'd',
		ArrayLong = 'a',
		LogicalLong = 'l',
		MemoLong = 'h'
	}
	// ReSharper restore CharImplicitlyConvertedToNumeric
}