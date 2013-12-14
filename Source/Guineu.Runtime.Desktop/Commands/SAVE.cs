using System;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
    class SAVE : ICommand
    {
        ExpressionBase file;
        ExpressionBase memo;

        public void Compile(CodeBlock code)
        {
            var comp = new Compiler(null, code);

            Token nextToken = code.Reader.PeekToken();
            do
            {
                switch (nextToken)
                {
                    case Token.MEMO:
                        code.Reader.ReadToken();
                        memo = comp.GetCompiledExpression();
                        break;
                    default:
                        file = comp.GetCompiledExpression();
                        break;
                }
                nextToken = code.Reader.PeekToken();
            } while (nextToken != Token.CmdEnd);
        }

        public void Do(CallingContext ctx, ref Int32 nextLine)
        {
            if (file != null)
            {
                String name = file.GetString(ctx);
                using (Stream fs = GuineuInstance.FileMgr.Open(name, FileMode.Create))
                    WriteMemFile(ctx, fs);
            }
            else
            {
                String memo = this.memo.GetString(ctx);
                using (Stream fs = new MemoryStream(GuineuInstance.CurrentCp.GetBytes(memo)))
                {
                    fs.WriteByte(0x4D);
                    fs.WriteByte(0x0F);
                    WriteMemFile(ctx, fs);
                }
            }

        }

        static void WriteMemFile(CallingContext ctx, Stream s)
        {
            using (var bw = new BinaryWriter(s, GuineuInstance.CurrentCp))
            {
                MemberList locals = ctx.Locals;
                for (Int32 i = 0; i < locals.Count; i++)
                {
                    Nti name = locals.GetMemberNameByPosition(i);
                    Member mbr = locals.GetMember(name);
                    if (mbr is ArrayMember)
                        WriteArray(bw, name, mbr as ArrayMember);
                    else
                        if (mbr is ValueMember)
                        {
                            var vm = mbr as ValueMember;
                            WriteVariant(bw, name, vm.Get(), false);
                        }
                }
                bw.Write((Byte)0x1A);
            }
        }

        private static void WriteArray(BinaryWriter bw, Nti name, ArrayMember arr)
        {
            if (arr.Dimensions > 1)
            {
                WriteArrayHeader(bw, name, (int)arr.Dimension1, (int)arr.Dimension2);
                for (Int32 x = 1; x <= arr.Dimension1; x++)
                    for (Int32 u = 1; u <= arr.Dimension2; u++)
                        WriteElement(bw, name, arr, x, u);
            }
            else
            {
                WriteArrayHeader(bw, name, (int)arr.Dimension2, 0);
                for (Int32 u = 1; u <= arr.Dimension2; u++)
                    WriteElement(bw, name, arr, 1, u);
            }
        }

        static void WriteElement(BinaryWriter bw, Nti name, ArrayMember arr, int d1, int d2)
        {
            ValueMember vm = arr.Locate(d1, d2);
            Variant value = vm.Get();
            WriteVariant(bw, name, value, true);
        }


        static void WriteVariant(BinaryWriter bw, Nti name, Variant value, bool isArr)
        {
            if (CanBeSaved(value))
            {
                MemFileType type = GetTypeOfVariant(value);
                var hdr = new MemFileRecordHeader(name.ToString(), value.Length(), value.Decimals, type, isArr);
                hdr.Write(bw);
                WriteValue(bw, value, type);
            }
        }

        private static void WriteValue(BinaryWriter bw, Variant value, MemFileType type)
        {
            switch (type)
            {
                case MemFileType.NumericLong:
                case MemFileType.Numeric:
                    bw.Write((Double)value);
                    break;

                case MemFileType.Character:
                case MemFileType.CharacterLong:
                case MemFileType.Memo:
                case MemFileType.MemoLong:
                    bw.Write(((String)value).ToCharArray());
                    bw.Write('\0');
                    break;

                case MemFileType.DateTime:
                case MemFileType.Date:
                case MemFileType.DateTimeLong:
                case MemFileType.DateLong:
                    var dt = new DateTime(2004, 5, 22);
                    DateTime dtV = value;
                    var x = dtV - dt;
                    Int32 days = x.Days;
                    days = days + 0x2856E9C;
                    Int32 sec = dtV.Second + 60 * dtV.Minute + 3600 * dtV.Hour;

                    var y = (sec * (0x7FFFFFFF / 86400)) + 2953;
                    Int32 i1 = (days << 31) + y;
                    Int32 i2 = ((days & 0x7fffffff) >> 1) + 0x40000000;

                    bw.Write(i1);
                    bw.Write(i2);
                    break;

                case MemFileType.Logical:
                case MemFileType.LogicalLong:
                    bw.Write((Boolean)value);
                    break;

                default:
                    break;
            }
        }

        static void WriteArrayHeader(BinaryWriter bw, Nti name, Int32 d1, Int32 d2)
        {
            var hdr = new MemFileRecordHeader(name.ToString(), 0, 0, MemFileType.Array, false);
            hdr.Write(bw);
            bw.Write((UInt16)d1);
            bw.Write((UInt16)d2);
        }

        private static Boolean CanBeSaved(Variant value)
        {
            switch (value.Type)
            {
                case VariantType.Object:
                case VariantType.Unknown:
                case VariantType.Binary:
                case VariantType.Null:
                    return false;
                default:
                    return true;
            }
        }

        private static MemFileType GetTypeOfVariant(Variant value)
        {
            switch (value.Type)
            {
                case VariantType.Integer:
                case VariantType.Number:
                    return MemFileType.Numeric;

                case VariantType.Logical:
                    return MemFileType.Logical;

                case VariantType.Character:
                    if (value.Length() > 254)
                        return MemFileType.Memo;
                    return MemFileType.Character;

                case VariantType.Date:
                    return MemFileType.Date;

                case VariantType.DateTime:
                    return MemFileType.DateTime;

                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }
    }

    class MemFileRecordHeader
    {
        readonly String Name;
        readonly Int32 Length;
        readonly Int32 Decimals;
        readonly MemFileType Type;
        readonly Boolean IsArrayElement;
        readonly Boolean IsLong;

        public MemFileRecordHeader(String name, Int32 length, Int32 decimals, MemFileType type, Boolean isArray)
        {
            this.Name = name;
            Length = length;
            Decimals = decimals;
            Type = type;
            IsArrayElement = isArray;
            IsLong = this.Name.Length > 10;
        }

        public void Write(BinaryWriter bw)
        {
            String nameShort = (IsLong ? "" : Name).PadRight(11, '\0');
            Int32 lengthShort = Length;
            Int32 lengthLong = 0;

            if (Type == MemFileType.Character)
                lengthShort++;

            if (Type == MemFileType.Memo)
            {
                lengthLong = lengthShort + 1;
                lengthShort = 0;
            }

            WriteHeader(bw, nameShort, lengthShort, lengthLong);
            WriteLongName(bw);
        }

        private void WriteLongName(BinaryWriter bw)
        {
            if (IsLong)
            {
                bw.Write((Int16)Name.Length);
                bw.Write(Name.ToCharArray());
            }
        }

        private void WriteHeader(BinaryWriter bw, String nameShort, Int32 lengthShort, Int32 lengthLong)
        {
            bw.Write(nameShort.ToCharArray());
            if (IsLong)
                bw.Write((Byte)ConvertToLong(Type));
            else
                bw.Write((Byte)Type);
            bw.Write(lengthLong);
            bw.Write((Byte)lengthShort);
            bw.Write((Byte)Decimals);
            bw.Write(0);
            bw.Write((Int16)0);
            if (IsArrayElement)
                bw.Write((Int16)0);
            else
                bw.Write((Int16)0x0301);
            bw.Write(0);
            bw.Write((Int16)0);
        }

        private static MemFileType ConvertToLong(MemFileType type)
        {
            switch (type)
            {
                case MemFileType.Numeric:
                    return MemFileType.NumericLong;
                case MemFileType.Character:
                    return MemFileType.CharacterLong;
                case MemFileType.DateTime:
                    return MemFileType.DateTimeLong;
                case MemFileType.Date:
                    return MemFileType.DateLong;
                case MemFileType.Array:
                    return MemFileType.ArrayLong;
                case MemFileType.Logical:
                    return MemFileType.LogicalLong;
                case MemFileType.Memo:
                    return MemFileType.MemoLong;
                default:
                    return type;
            }
        }
    }

}