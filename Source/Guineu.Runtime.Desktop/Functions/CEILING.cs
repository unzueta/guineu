using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class CEILING : ExpressionBase
	{
		ExpressionBase m_Value;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					m_Value = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant retVal = new Variant(GetInt(context), 20);
			return retVal;
		}

		internal override int GetInt(CallingContext context)
		{
			return (int)Math.Ceiling(m_Value.GetDouble(context));
		}
	}

}