using System;
using Guineu.Data.Engines.Dbf;

namespace Guineu.Data.Dbf
{
	partial class LockManager
	{
		readonly DbfTable Tbl;

		//===================================================================================
		const Int64 LockAreaPosition = 2147483646;

		//===================================================================================
		internal LockManager(DbfTable tbl)
		{
			Tbl = tbl;
		}

		//===================================================================================
		internal bool LockRecord(Int64 recNo)
		{
			return LockRange(LockAreaPosition - recNo, 1);
		}

		//===================================================================================
		internal bool LockHeader()
		{
			return LockRange(LockAreaPosition, 1);
		}

		//===================================================================================
		internal bool LockTable()
		{
			return LockRange( LockAreaPosition - LockAreaLength()+1, LockAreaLength());
		}

		//===================================================================================
		internal void UnlockRecord(Int64 recNo)
		{
			UnlockRange(LockAreaPosition - recNo, 1);
		}

		//===================================================================================
		internal void UnlockHeader()
		{
			UnlockRange(LockAreaPosition, 1);
		}

		//===================================================================================
		internal void UnlockTable()
		{
			UnlockRange(LockAreaPosition - LockAreaLength() + 1, LockAreaLength());
		}

		//===================================================================================
		/// <summary>
		/// Returns the maximum number of records that this table may contain.
		/// </summary>
		/// <remarks>
		/// This routine uses the same algorithm as Visual FoxPro to calculate the number
		/// of records. The maximum file size is 2GB. This space is divides among table
		/// header, locking flag for the table, locking flag for the header, locking flag
		/// for each record and the actual record data.
		/// </remarks>
		/// <returns></returns>
		Int64 LockAreaLength()
		{
			Int64 availableSpace = 2 ^ 31 - Tbl.Header.GetHeaderLength() - 2;
			Int64 maxLockLength = availableSpace / (Tbl.Header.GetRecordSize() + 1);
			return maxLockLength;
		}
	}
}
