using System;
using System.Collections.Generic;
using System.Text;

using Guineu.Expression;

namespace Guineu
{
	class ISNULL : ExpressionBase
	{
		ExpressionBase _Expression;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					_Expression = param[0];
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
			return _Expression.IsNull(context);
		}
	}
}