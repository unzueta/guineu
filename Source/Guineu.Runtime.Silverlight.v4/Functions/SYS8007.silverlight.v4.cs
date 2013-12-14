using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu
{
	static partial class SYS8007
	{
		public static String getString(CallingContext context, List<ExpressionBase> param)
		{
			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					ExpressionBase number = param[1];
					return MakePhoneCall(context, number);
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		public static String MakePhoneCall(CallingContext context, ExpressionBase number)
		{
			return "0";
		}

	}

}