using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYS8000 : ISys
	{
		/// <summary>
		/// Enables or disables Unicode support.
		/// </summary>
		/// <returns></returns>
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			// Query the current state
		    var retVal = GuineuInstance.UseUnicode ? "1" : "0";

			// Change the current state
			if (param.Count >= 2)
			{
				GuineuInstance.UseUnicode = param[1].GetBool(context);
			}

			return retVal;
		}
	}

}