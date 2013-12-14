using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYS2015 : ISys
	{
		static Int32 lastValue;
		static Int32 lastDay;

		// Description of SYS(2015) format: Frank Camp
		public string getString(CallingContext context, List<ExpressionBase> param)
		{
			return GetString();
		}

		public static string GetString()
		{
			DateTime baseVal = DateTime.Now;
			Int32 day = CalculateDays(baseVal);
			var milliSeconds = (Int32)baseVal.TimeOfDay.TotalMilliseconds;

			if (lastDay == day && milliSeconds <= lastValue)
				milliSeconds = lastValue + 1;
			lastValue = milliSeconds;
			lastDay = day;

			String str = "_" + ConvertToBase(day, 36).PadRight(3, '0') + ConvertToBase(milliSeconds, 36).PadRight(6, '0');
			return str;
		}

		static Int32 CalculateDays(DateTime baseVal)
		{
			return 367 * (baseVal.Year % 100) + baseVal.DayOfYear;
		}


		// Source: http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=415703&SiteID=1
		static public String ConvertToBase(int num, int nbase)
		{
			const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			// check if we can convert to another base
			if (nbase < 2 || nbase > chars.Length)
				return "";

		    String newNumber = "";

			// in r we have the offset of the char that was converted to the new base
			while (num >= nbase)
			{
				int r = num % nbase;
				newNumber = chars[r] + newNumber;
				num = num / nbase;
			}
			// the last number to convert
			newNumber = chars[num] + newNumber;

			return newNumber;
		}
	}

}