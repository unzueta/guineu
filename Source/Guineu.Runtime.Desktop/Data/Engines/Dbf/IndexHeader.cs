using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu.Data.Dbf
{
	public class IndexHeader : IDisposable
	{
		BinaryReader Reader;

		internal Int32 RootNode;
		Int32 _FreeNodeList;
		Int32 _CurrentTimeStamp;
		internal Int16 KeyLength;
		internal IndexOptions _Options;
		Byte _IndexSignature;

		Int32 reserved1;
		Int32 reserved2;
		Int32 reserved3;
		Int32 reserved4;
		Int32 reserved5;
		Byte[] reserved6;

		Boolean ascending;
		Int16 reserved7;
		Int16 lenFor;
		Int16 reserved8;
		Int16 lenKey;

		Byte[] key;
		Byte[] forClause;

		CodeBlock keyExpression;
		CodeBlock forExpression;

		public IndexHeader(Byte[] buffer)
		{
			Refresh(buffer);
		}

		internal void Refresh(Byte[] buffer)
		{
			Reader = new BinaryReader(new MemoryStream(buffer));
			RootNode = Reader.ReadInt32();
			_FreeNodeList = Reader.ReadInt32();
			_CurrentTimeStamp = Reader.ReadInt32();
			KeyLength = Reader.ReadInt16();
			_Options = (IndexOptions)Reader.ReadByte();
			if (buffer.Length > 16)
			{
				_IndexSignature = Reader.ReadByte();
				reserved1 = Reader.ReadInt32();
				reserved2 = Reader.ReadInt32();
				reserved3 = Reader.ReadInt32();
				reserved4 = Reader.ReadInt32();
				reserved5 = Reader.ReadInt32();
				reserved6 = Reader.ReadBytes(466);
				ascending = (Reader.ReadInt16() == 0);
				reserved7 = Reader.ReadInt16();
				lenFor = Reader.ReadInt16();
				reserved8 = Reader.ReadInt16();
				lenKey = Reader.ReadInt16();
				key = Reader.ReadBytes(lenKey);
				forClause = Reader.ReadBytes(lenFor);
				keyExpression = Tokenizer.Expression(key);
				forExpression = Tokenizer.Expression(key);
			}
		}

		internal Variant Get(CallingContext context)
		{
			// TODO: Make this a property of the tag.
			ExpressionBase ex;
			Compiler comp = new Compiler(context, keyExpression);
			ex = comp.GetCompiledExpression();
			return ex.GetVariant(context);
		}


		private bool disposed = false;

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
					Reader.Close();
				}
			disposed = true;
		}   
	}
}
