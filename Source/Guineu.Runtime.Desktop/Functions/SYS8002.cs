using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYS8002 : ISys
	{
		/// <summary>
		/// Enables or disables exceptions for unknown token
		/// </summary>
		/// <returns></returns>
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			// Query the current state
		    string retVal = GuineuInstance.IgnoreUnknownTokens ? "1" : "0";

			// Change the current state
			if (param.Count >= 2)
			{
				GuineuInstance.IgnoreUnknownTokens = param[1].GetBool(context);
			}

			return retVal;
		}
	}

}