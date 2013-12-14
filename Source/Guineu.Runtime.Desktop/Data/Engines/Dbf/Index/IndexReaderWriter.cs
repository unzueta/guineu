using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Data.Dbf.Index
{

	class IndexReaderWriter
	{
		#region public construction methods
		public IndexReaderWriter(IndexNodeReader reader, IndexHeader theHeader)
		{
			nodeReader = reader;
			header = theHeader;
		}
		#endregion 

		/// <summary>
		/// Returns the header node of the specified index
		/// </summary>
		/// <returns></returns>
		public IndexNode GetHeader()
		{
			IndexNode node = nodeReader.GetNode(header.RootNode, header.KeyLength);
			return node;
		}

		/// <summary>
		/// Returns a node from the CDX file
		/// </summary>
		/// <param Name="position"></param>
		/// <returns></returns>
		public IndexNode GetNode( Int64 position)
		{
			IndexNode node = nodeReader.GetNode( (Int32) position, header.KeyLength);
			return node;
		}
		
		#region private members
		IndexNodeReader nodeReader;
		IndexHeader header;
		#endregion 

	}
}
