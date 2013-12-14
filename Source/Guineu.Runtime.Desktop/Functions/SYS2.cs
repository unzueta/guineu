using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYS2 : ISys
	{
		public string getString(CallingContext context, List<ExpressionBase> param)
		{
			try
			{
				var sec = new SECONDS();
				return sec.GetInt(context).ToString();
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}

}