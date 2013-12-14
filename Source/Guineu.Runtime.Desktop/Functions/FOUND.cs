using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
	class FOUND : ExpressionBase
	{
		ExpressionBase alias;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				case 1:
					alias = param[0];
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
			ICursor csr = context.GetCursor(alias);
			if (csr == null)
				return false;
			
			return csr.Found;
		}
	}

}