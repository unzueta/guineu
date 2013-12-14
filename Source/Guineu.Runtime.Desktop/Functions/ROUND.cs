using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class ROUND : ExpressionBase
	{ 
		ExpressionBase expression;
		ExpressionBase dec;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					expression = param[0];
					dec = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant value = expression.GetVariant(context);
			if(value.Type!=VariantType.Number && value.Type!=VariantType.Integer)
				throw new ErrorException(ErrorCodes.InvalidArgument);
			if (value.IsNull)
				return new Variant(value.Type, true);

			value = dec.GetVariant(context);
			if (value.Type != VariantType.Number && value.Type != VariantType.Integer)
				throw new ErrorException(ErrorCodes.InvalidArgument);
			if (value.IsNull)
				return new Variant(value.Type, true);

			var retVal = new Variant(GetDouble(context), 20, 10);
			return retVal;
		}

		internal override double GetDouble(CallingContext context)
		{
			int nDec = dec.GetInt(context);
			if(nDec<0)
				return 0.0;

			double fVal = expression.GetDouble(context);
			bool neg = fVal<0 ;
			double nPrec = Math.Pow(10, nDec);
			if (neg)
				fVal = -fVal ;

			// TODO : add a desktop and a compact to use this in desktop :
			// return Math.Round(fVal, nDec, MidpointRounding.AwayFromZero)

			fVal = Math.Floor(fVal * nPrec + 0.5 + 1e-18) / nPrec;
			return (neg)?-fVal:fVal ;
		}
	}

}