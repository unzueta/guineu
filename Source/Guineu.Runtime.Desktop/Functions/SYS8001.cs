using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYS8001 : ISys
	{
		/// <summary>
		/// Defines how file names are handled.
		/// </summary>
		/// <returns></returns>
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			var retVal = ((Int32)GuineuInstance.SetPathHandling).ToString();
			if (param.Count >= 2)
			{
				GuineuInstance.SetPathHandling = (PathHandling) param[1].GetInt(context);
			}

			return retVal;
		}
	}

}