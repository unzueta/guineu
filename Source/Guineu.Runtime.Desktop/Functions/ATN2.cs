using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class ATN2 : ExpressionBase
	{
		ExpressionBase m_Value1;
		ExpressionBase m_Value2;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					m_Value1 = param[0];
					m_Value2 = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant retVal = new Variant(GetDouble(context), 20, 10);
			return retVal;
		}

		internal override double GetDouble(CallingContext context)
		{
			return Math.Atan2(m_Value1.GetDouble(context), m_Value2.GetDouble(context));
		}
	}

}