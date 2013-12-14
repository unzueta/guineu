using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class EVALUATE : ExpressionBase
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
			CodeBlock cb = Tokenizer.Expression(_Expression.GetString(context));
			Compiler comp = new Compiler(context, cb);
			Variant var = comp.GetCompiledExpression().GetVariant(context);
			return var;
		}
	}

}