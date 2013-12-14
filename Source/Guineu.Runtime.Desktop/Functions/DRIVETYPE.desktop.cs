using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	partial class DRIVETYPE : ExpressionBase
	{
		internal override int GetInt(CallingContext context)
		{
			string cFullName = _Path.GetString(context);
			if (!Path.IsPathRooted(cFullName))
				return 1;

			DriveInfo di = new DriveInfo(cFullName);
			switch (di.DriveType)
			{
				case DriveType.CDRom:
					return 5;
				case DriveType.Fixed:
					return 3;
				case DriveType.Network:
					return 4;
				case DriveType.NoRootDirectory:
					return 1;
				case DriveType.Ram:
					return 6;
				case DriveType.Removable:
					return 2;
				default:
					return 1;
			}
		}

	}
}
