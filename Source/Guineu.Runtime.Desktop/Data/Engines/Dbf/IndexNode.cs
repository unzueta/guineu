using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Guineu.Data.Dbf
{
	abstract public class IndexNode : IDisposable
	{
		protected BinaryReader Reader;
		protected NodeType Type;
		protected Int16 KeyCount;
		protected Int32 LeftNode;
		protected Int32 RightNode;

		/// <summary>
		/// Length of a key value in bytes
		/// </summary>
		protected Int16 KeyLength;

		protected IndexNode(Byte[] buffer, Int16 keyLength)
		{
			Reader = new BinaryReader(new MemoryStream(buffer));
			Type = (NodeType)Reader.ReadInt16();
			KeyCount = Reader.ReadInt16();
			LeftNode = Reader.ReadInt32();
			RightNode = Reader.ReadInt32();
			KeyLength = keyLength;
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

	public enum NodeType
	{
		IndexNode = 0,
		RootNode = 1,
		LeafNode = 2
	}

}
