using System;
using System.Runtime.InteropServices;

namespace Guineu.Data.Dbf
{
	partial class LockManager
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct NativeOverlapped
		{
			private IntPtr InternalLow;
			private IntPtr InternalHigh;
			public long Offset;
			public IntPtr EventHandle;
		}

		[DllImport("coredll.dll", EntryPoint = "LockFileEx", SetLastError = true)]
		static extern bool LockFileEx(IntPtr hFile, uint dwFlags, uint dwReserved,
		                              uint nNumberOfBytesToLockLow, uint nNumberOfBytesToLockHigh,
		                              [In] ref NativeOverlapped lpOverlapped);

		//===================================================================================
		/// <summary>
		/// Locks a range in the table
		/// </summary>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		bool LockRange(Int64 start, Int64 length)
		{
			//NativeOverlapped o = new NativeOverlapped();
			//o.Offset = (Int32) start;
			//o.EventHandle = (IntPtr) 0;
			//FileStream fs = Tbl.Stream as FileStream;
			//if (fs != null)
			//{
			//  LockFileEx(fs.);
			//}
			return true;
		}

		//===================================================================================
		/// <summary>
		/// Unlocks a range in the table
		/// </summary>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		void UnlockRange(Int64 start, Int64 length)
		{
	}
	}
}
