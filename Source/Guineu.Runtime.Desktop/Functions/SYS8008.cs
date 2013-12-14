using System;
using Guineu.Expression;
using System.Collections.Generic;

namespace Guineu.Functions
{
	/// <summary>
	/// Enables additional base classes.
	/// </summary>
	partial class SYS8008 : ISys
	{
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					var n = param[1].ToNti(context);
					return RegisterBaseClass(n) ? "1" : "0";
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}
	}

}