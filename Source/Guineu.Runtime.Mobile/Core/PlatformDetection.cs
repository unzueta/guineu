// based on: http://blogs.msdn.com/b/netcfteam/archive/2006/09/15/756755.aspx

using System;
using System.Text;

namespace Guineu.Core
{
	public class PlatformDetection
	{
		static string GetOemInfo()
		{
			var oemInfo = new StringBuilder(50);
			if (PInvoke.SystemParametersInfo4Strings((uint)PInvoke.SystemParametersInfoActions.SPI_GETOEMINFO,
					(uint)oemInfo.Capacity, oemInfo, 0) == 0)
				throw new Exception("Error getting OEM info.");
			return oemInfo.ToString();
		}

		private const string MicrosoftEmulatorOemValue = "Microsoft DeviceEmulator";
		public static bool IsEmulator()
		{
			return GetOemInfo() == MicrosoftEmulatorOemValue;
		}
	}
}