using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	/// <summary>
	/// Set time
	/// </summary>
	partial class SYS8013 : ISys
	{
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			switch (param.Count)
			{
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					var time = param[1].GetVariant(context);
					SetTime(time);
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			return "";
		}

		static partial void SetTime(DateTime time);
	}
}