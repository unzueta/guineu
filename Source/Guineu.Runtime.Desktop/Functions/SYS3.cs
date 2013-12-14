using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYS3 : ISys
	{
		public string getString(CallingContext context, List<ExpressionBase> param)
		{
			try
			{
				// TODO : use the same imp than seconds but using milliseconds.
				var dt = DateTime.Now;
				return ((int)Math.Round(dt.TimeOfDay.TotalMilliseconds, 0)).ToString();
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}

}