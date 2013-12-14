using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	 class SYS8007 : ISys
	{
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					var number = param[1];
					return MakePhoneCall(context, number);
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		public static String MakePhoneCall(CallingContext context, ExpressionBase number)
		{
			var destination = number.GetString(context);
			Boolean result = OpenNETCF.Phone.Phone.MakeCall(destination);
			return result ? "1" : "0";
		}

	}

}