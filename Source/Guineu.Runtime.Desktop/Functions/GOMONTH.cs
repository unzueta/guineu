using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class GOMONTH : ExpressionBase
	{
		ExpressionBase date;
		ExpressionBase month;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					date = param[0];
					month = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant val = date.GetVariant(context);
			DateTime dt = GetDateTime(context, val);
			return new Variant(dt, val.Type);
		}

		internal DateTime GetDateTime(CallingContext context, Variant value)
		{
			DateTime dt = value;
			return dt.AddMonths(month.GetInt(context));
		}

	}
}
