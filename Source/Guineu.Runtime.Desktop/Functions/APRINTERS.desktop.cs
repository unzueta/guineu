using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Management;
using Guineu.Expression;

namespace Guineu
{
	partial class APRINTERS : ExpressionBase
	{
		static ManagementObjectCollection _Printers;
		static Int32 _LastCount;

		private Variant GetExtendedInfo(ArrayMember arr, CallingContext context)
		{
			Int32 cnt = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count;
			arr.Dimension(cnt, 5);

			Int32 item = 0;
			if (_Printers == null || _LastCount != cnt)
			{
				_LastCount = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count;
				_Printers = new ManagementClass("Win32_Printer").GetInstances();
			}
			foreach (ManagementObject printer in _Printers)
			{
				item++;
				arr.Locate(item, 1).SetString(printer["Name"].ToString());
				arr.Locate(item, 2).SetString(printer["PortName"].ToString());
				if (printer["DriverName"] == null)
					arr.Locate(item, 3).SetString("");
				else
					arr.Locate(item, 3).SetString(printer["DriverName"].ToString());
				if (printer["Location"] == null)
					arr.Locate(item, 4).SetString("");
				else
					arr.Locate(item, 4).SetString(printer["Location"].ToString());
				if (printer["Description"] == null)
					arr.Locate(item, 5).SetString("");
				else
					arr.Locate(item, 5).SetString(printer["Description"].ToString());
			}

			return new Variant(cnt, 11);
		}

		private Variant GetSimpleInfo(ArrayMember arr, CallingContext context)
		{
			Int32 cnt = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count;
			arr.Dimension(cnt, 2);

			Int32 item = 0;
			ManagementObjectCollection printers = new ManagementClass("Win32_Printer").GetInstances();
			foreach (ManagementObject printer in printers)
			{
				item++;
				arr.Locate(item, 1).SetString(printer["Name"].ToString());
				arr.Locate(item, 2).SetString(printer["PortName"].ToString());
			}

			return new Variant(cnt, 11);
		}
		private Variant GetFastInfo(ArrayMember arr, CallingContext context)
		{
			Int32 cnt = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count;
			arr.Dimension(cnt, 1);

			int i = 1;
			foreach (string PrinterName in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
			{
				arr.Locate(i, 1).SetString(PrinterName);
				i++;
			}

			return new Variant(cnt, 11);
		}
	}
}