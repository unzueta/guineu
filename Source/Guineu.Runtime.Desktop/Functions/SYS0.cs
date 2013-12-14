using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYS0 : ISys
	{
		public string getString(CallingContext context, List<ExpressionBase> param)
		{
			try
			{
			    string retVal = Environment.MachineName + " # " + Environment.UserName;
			    return retVal;
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}

}