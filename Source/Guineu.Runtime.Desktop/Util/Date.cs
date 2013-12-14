using System;

using System.Collections.Generic;
using System.Text;

namespace Guineu.Util
{
	static class Date
	{
		public static Int32 ToJulian(DateTime value)
		{

			int y = value.Year;
			int m = value.Month;
			int day = value.Day;
			int ijulian;
			int IGREG = 15 + 31 * (10 + 12 * 1582); // Greg.Calendar 10/15/1582
			int adj;

			if (y < 0) y = y + 1;
			if (m > 2)
				m = m + 1;
			else
			{
				y = y - 1;
				m = m + 13;
			}

			ijulian = (int)(365.25 * y) + (int)(30.6001 * m) + day + 1720995;

			if (day + 31 * (m + 12 * y) >= IGREG)
			{ // change for Gregorian calendar
				adj = y / 100;
				ijulian = ijulian + 2 - adj + adj / 4;
			}


			Int32 dayPart = ijulian;
			return dayPart;
		}

	}
}
