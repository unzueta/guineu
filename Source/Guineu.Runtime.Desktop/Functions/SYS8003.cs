using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYS8003 : ISys
	{
		/// <summary>
		/// Enables or disables .NET compiler support.
		/// </summary>
		/// <returns></returns>
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			// Query the current state
		    string retVal = GuineuInstance.CallDebugger ? "1" : "0";

			// Change the current state
			if (param.Count >= 2)
			{
				GuineuInstance.CallDebugger = param[1].GetBool(context);
			}

			return retVal;
		}
	}

}