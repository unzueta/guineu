using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Data.Engines.Dbf.Index;

namespace Guineu.Data.Dbf.Index
{
	class KeyList
	{
		#region public construction methods

		public KeyList(IndexReaderWriter source)
		{
			indexSource = source;
		}

		#endregion

		/// <summary>
		/// Locates and returns the key from the index that is the closest match 
		/// to the key passed into this method.
		/// </summary>
		/// <param Name="item"></param>
		/// <returns>
		/// The key that is the closest match. Find attempts to locate a key
		/// with the exact same byte code. If there is any, the record with the 
		/// record number equal or higher than the passed one is returned. Lacking
		/// a precise match, return the first item whose byte code is greater. 
		/// If no key is found at all, an empty KeyItem is returned.
		/// </returns>
		public KeyItem Find(KeyItem item)
		{
			//
			// What to search
			//
			Byte[] data = item.GetBytes();
			Int64 recNo = item.GetRecord();
			//
			// Walk the tree down to the lowest level. Only exterior nodes point to
			// records. All interior nodes point to other nodes in the tree.
			//
			IndexNodeExterior extNode = FindExteriorNode(data, recNo);
			if (extNode == null)
				return item.New();
			//
			// Inside the exterior node, locate the proper entry
			//
			SeekResult found = extNode.Locate(data, (Int32) recNo, true);
			KeyItem retVal = item.New(extNode, found.Index);
			//
			return retVal;
		}
		public void Add(KeyItem item)
		{
		}
		public void Remove(KeyItem item)
		{
			//
			// What to search
			//
			Byte[] data = item.GetBytes();
			Int64 recNo = item.GetRecord();
			//
			// Walk the tree down to the lowest level. Only exterior nodes point to
			// records. All interior nodes point to other nodes in the tree.
			//
			IndexNodeExterior extNode = FindExteriorNode(data, recNo);
			if (extNode != null)
			{
				SeekResult found = extNode.Locate(data, (Int32)recNo, true);
				if (found.Found)
					extNode.RemoveKey(found.Index);
			}
		}

		#region private methods
		/// <summary>
		/// Searches for the first occurrence of a particular value in the
		/// index tree.
		/// </summary>
		/// <param Name="value"></param>
		/// <param Name="exact">
		/// true: Compares the entire value
		/// false: Compares only up to the lenth of value.
		/// </param>
		/// <returns></returns>
		private IndexNodeExterior FindExteriorNode(Byte[] value, Int64 recNo)
		{
			IndexNode node = indexSource.GetHeader();
			while (node is IndexNodeInterior)
			{
				Int32 nextNode = ((IndexNodeInterior)node).Locate(value, (Int32) recNo);
				if (nextNode == 0)
					return null;
				node = indexSource.GetNode(nextNode);
			}
			return (IndexNodeExterior)node;
		}
		#endregion

		#region private Members
		IndexReaderWriter indexSource;
		#endregion 
	}
}
