using System;
using System.Collections.Generic;
using System.Text;

using Guineu.Expression;

namespace Guineu
{
	partial class GETENV : ExpressionBase
	{
		ExpressionBase envVariable;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					envVariable = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			return new Variant(GetString(context));
		}
	}
}