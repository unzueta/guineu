using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Data;

namespace Guineu.Data.Dbf
{
	partial class LockManager
	{
		//===================================================================================
		/// <summary>
		/// Locks a range in the table
		/// </summary>
		/// <param Name="start"></param>
		/// <param Name="length"></param>
		/// <returns></returns>
		bool LockRange(Int64 start, Int64 length)
		{
			var fs = Tbl.Stream as FileStream;
			if (fs != null)
			{
				// TODO: Create a list of all active locks.
				// TODO: Handle SET MULTILOCK
				// TODO: Handle SET REPROCESS
				try
				{
					fs.Lock(start, length);
				}
				catch (IOException)
				{
					return false;
				}
			}
			return true;
		}

		//===================================================================================
		/// <summary>
		/// Unlocks a range in the table
		/// </summary>
		/// <param Name="start"></param>
		/// <param Name="length"></param>
		/// <returns></returns>
		void UnlockRange(Int64 start, Int64 length)
		{
			var fs = Tbl.Stream as FileStream;
			if (fs != null)
				fs.Unlock(start, length);
		}
	}
}
