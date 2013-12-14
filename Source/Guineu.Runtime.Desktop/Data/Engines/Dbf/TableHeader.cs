using System;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu.Data.Dbf
{
	class TableHeader
	{
		FileTypes fileType;
		UpdateDate lastUpdate;
		Int64 recCount;
		Int32 length;
		Int32 recSize;
		internal TableFlags flags;
		Int32 codepage;
		FieldDefinition fields;
		String backLink;

		/// <summary>
		/// Initializes the header from a stream.
		/// </summary>
		/// <param Name="s">Table stream</param>
		public TableHeader(Stream s)
		{
			s.Seek(0, SeekOrigin.Begin);
			BinaryReader r = new BinaryReader(s);
			ReadHeader(r);
		}

		public void RefreshHeader(Stream s)
		{
			s.Seek(0, SeekOrigin.Begin);
			BinaryReader r = new BinaryReader(s);
			ReadCoreHeader(r);
		}

		private void ReadHeader(BinaryReader r)
		{
			ReadCoreHeader(r);
			ReadFields(r);
			if (fileType == FileTypes.VFP
					|| fileType == FileTypes.VFP8
					|| fileType == FileTypes.VFP9)
			{
				Encoding decoder = Encoding.GetEncoding(codepage);
				r.ReadByte();
				char[] charValue = GuineuInstance.CurrentCp.GetChars(r.ReadBytes(260));
				backLink = new String(charValue).Trim('\0');
			}
		}

		private void ReadCoreHeader(BinaryReader r)
		{
			fileType = (FileTypes)r.ReadByte();
			lastUpdate.Year = r.ReadByte();
			lastUpdate.Month = r.ReadByte();
			lastUpdate.Day = r.ReadByte();
			recCount = r.ReadUInt32();
			length = r.ReadUInt16();
			recSize = r.ReadUInt16();
			r.ReadBytes(16);
			flags = (TableFlags)r.ReadByte();
			codepage = TranslateCodePage(r.ReadByte());
			r.ReadBytes(2);
		}

		/// <summary>
		/// Translates the code page stored in the table into an actual codepage.
		/// </summary>
		/// <remarks>
		/// Source: http://support.microsoft.com/?scid=kb%3Ben-us%3B129631&x=11&y=16
		/// </remarks>
		Int32 TranslateCodePage(Byte codePageMark)
		{
			switch (codePageMark)
			{
				case 0x01:
					return 437;
				case 0x69:
					return 620;
				case 0x6A:
					return 737;
				case 0x02:
					return 850;
				case 0x64:
					return 852;
				case 0x67:
					return 861;
				case 0x66:
					return 865;
				case 0x65:
					return 866;
				case 0x68:
					return 895;
				case 0x6B:
					return 857;
				case 0xC8:
					return 1250;
				case 0xC9:
					return 1251;
				case 0x03:
					return 1252;
				case 0xCB:
					return 1253;
				case 0x04:
					return 10000;
				case 0x98:
					return 10006;
				case 0x96:
					return 10007;
				case 0x97:
					return 10029;
				default:
					// TODO: Return CodePage= setting from Config.FPW or system code page
					return 1252;
			}
		}

		internal void UpdateRecordCount(BinaryWriter w, Int64 newCount)
		{
			w.Seek(1, SeekOrigin.Begin);
			w.Write((Byte)(DateTime.Now.Year % 100));
			w.Write((Byte)DateTime.Now.Month);
			w.Write((Byte)DateTime.Now.Day);
			w.Write((Int32)newCount);
		}

		internal Encoding GetEncoder()
		{
			return Encoding.Default;
		}

		private void ReadFields(BinaryReader r)
		{
			Encoding decoder = GetEncoder();
			fields = new FieldDefinition(GetFieldCount(), r, decoder);
		}

		private Int32 GetFieldCount()
		{
			Int32 count;
			if (fileType == FileTypes.VFP
					|| fileType == FileTypes.VFP8
					|| fileType == FileTypes.VFP9)
			{
				count = (length - 32 - 264) / 32;
			}
			else
			{
				count = (length - 32) / 32;
			}
			return count;
		}

		internal FieldDefinition GetDefinition()
		{
			return fields;
		}

		/// <summary>
		/// Returns the definition of a particular field
		/// </summary>
		/// <param Name="fieldNo"></param>
		/// <returns></returns>
		public FieldDefinitionEntry this[Int32 fieldNo]
		{
			get { return fields[fieldNo]; }
		}

		public FieldDefinitionEntry this[Nti field]
		{
			get { return fields[field]; }
		}

		//===================================================================================
		public Int32 GetHeaderLength()
		{
			return length;
		}

		//===================================================================================
		public Int32 GetRecordSize()
		{
			return recSize;
		}

		//===================================================================================
		public Int64 GetRecCount()
		{
			return recCount;
		}
	}

	enum FileTypes : byte
	{
		FoxBase = 0x02,
		FoxBasePlus = 0x03,
		FoxBasePlusMemo = 0x83,	
		Fox2xMemo = 0xF5,
		FoxBase2 = 0xF2,
		
		VFP = 0x30, 
		VFP8 = 0x31,
		VFP9 = 0x32,

		DBase4 = 0x43,
		DBase4Sys = 0x63,
  	DBase4Memo = 0x8B,
		DBase4SQLMemo = 0xCB
	}

	struct UpdateDate
	{
		public Byte Year;
		public Byte Month;
		public Byte Day;
	}

	[Flags]
	enum TableFlags
	{
		HasCDX = 0x01,
		HasMemo = 0x02,
		IsDBC = 0x04
	}

}
