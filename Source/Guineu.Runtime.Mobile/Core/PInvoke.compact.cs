using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Guineu.Core
{
	static partial class PInvoke
	{
		[DllImport("coredll.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);

		[DllImport("coredll.dll", SetLastError = true)]
		public static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("Coredll.dll", EntryPoint = "SystemParametersInfoW", CharSet = CharSet.Unicode)]
		public static extern int SystemParametersInfo4Strings(uint uiAction, uint uiParam, StringBuilder pvParam, uint fWinIni);

		public enum SystemParametersInfoActions : uint
		{
			SPI_GETPLATFORMTYPE = 257, // this is used elsewhere for Smartphone/PocketPC detection
			SPI_GETOEMINFO = 258,
		}

	}
}