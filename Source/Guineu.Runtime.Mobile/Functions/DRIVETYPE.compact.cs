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
		{	// based on GetShortenedPath found on the web...
			string cFullName = _Path.GetString(context);
			try
			{
				//				if (cFullName.IndexOf(Path.VolumeSeparatorChar) > 0)
				//				{
				//					string cDrive = cFullName.Substring(0, path.IndexOf(Path.VolumeSeparatorChar) + 1);
				DirectoryInfo di = new DirectoryInfo(cFullName);
				// TODO ?? find a way to know if it's a LAN, remote shared or not
				return 6;
				//				}
			}
			catch (Exception)
			{
				throw new ErrorException(ErrorCodes.FileNotFound);
			}
		}

	}
}
