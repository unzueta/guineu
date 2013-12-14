using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYS1 : ISys
	{
		public string getString(CallingContext context, List<ExpressionBase> param)
		{
		    try
			{
			    var dt = DateTime.Today;
				// compute julian day
				var julian = ((1461 * (dt.Year + 4800 + (dt.Month - 14) / 12)) / 4 +
				              (367 * (dt.Month - 2 - 12 * ((dt.Month - 14) / 12))) / 12 -
				              (3 * ((dt.Year + 4900 + (dt.Month - 14) / 12) / 100)) / 4 +
				              dt.Day - 32075);

				var retVal = julian.ToString();
				return retVal;
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}

}