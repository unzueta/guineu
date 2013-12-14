using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Util;

namespace Guineu.Data.Dbf
{
	/// <summary>
	/// Provides access to index nodes. If possible we cache nodes.
	/// </summary>
	public class IndexNodeReader
	{
		Stream _Stream;
		IndexFile _Index;

		public IndexNodeReader(Stream stream, IndexFile file)
		{
			_Stream = stream;
			_Index = file;
		}

		/// <summary>
		/// Returns a node from the stream or the cache. 
		/// </summary>
		/// <param Name="node">Node position in the stream</param>
		/// <returns></returns>
		public IndexNode GetNode(Int32 position, Int16 keylength)
		{
			_Index.QuickRefreshHeader();
			return GetNodeFromStream(position,keylength);
		}

		public IndexHeader GetHeader( Int32 position)
		{
			_Index.QuickRefreshHeader();
			return GetHeaderFromStream(position);
		}

		private IndexNode GetNodeFromStream(Int32 position, Int16 keylength)
		{
			Byte[] buffer;
			lock (_Stream)
			{
				_Stream.Seek(position, SeekOrigin.Begin);
				buffer = StreamUtil.Read(_Stream, 0x200);
			}
			NodeType type = (NodeType)BitConverter.ToInt16(buffer, 0);
			IndexNode node;
			if (EnumUtil.IsSet((Int32)type, (Int32)NodeType.LeafNode))
				node = new IndexNodeExterior(buffer,keylength);
			else
				node = new IndexNodeInterior(buffer,keylength);
			return node;
		}

		private IndexHeader GetHeaderFromStream(Int32 position)
		{
			Byte[] buffer;
			lock (_Stream)
			{
				_Stream.Seek(position, SeekOrigin.Begin);
				buffer = StreamUtil.Read(_Stream, 0x400);
			}
			return new IndexHeader(buffer);
		}

	}
}
