using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class LOG10 : ExpressionBase
	{
		ExpressionBase value;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					value = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetDouble(context), 20, 10);
			return retVal;
		}

		internal override double GetDouble(CallingContext context)
		{
			return Math.Log10(value.GetDouble(context));
		}
	}

}