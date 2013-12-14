using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
	class EOF : ExpressionBase
	{
		ExpressionBase _Alias;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				case 1:
					_Alias = param[0];
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
			ICursor csr = context.DataSession[_Alias, context];
			if (csr == null)
				return false;
			else
				return csr.Eof();
		}
	}

}