using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class BITTEST : ExpressionBase
	{	// TODO : varbinary and blob
		ExpressionBase numExp1;
		ExpressionBase numExp2;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();

			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					numExp1 = param[0];
					numExp2 = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			return new Variant(GetBool(context));
		}

		internal override bool GetBool(CallingContext context)
		{
			int val1 = numExp1.GetInt(context);
			int val2 = numExp2.GetInt(context);
			try
			{
				val2 = (int)Math.Pow(2, val2);
			}
			catch (Exception)
			{
				throw new ErrorException(ErrorCodes.InvalidSubscript);
			}

			if((val1 & val2)!=0)
				return true;

			return false;
		}
	}
}