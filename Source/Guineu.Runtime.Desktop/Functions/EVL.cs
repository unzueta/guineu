using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class EVL : ExpressionBase
	{
		ExpressionBase m_Value1;
		ExpressionBase m_Value2;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
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


		override internal string GetString(CallingContext context)
		{
			Variant Val1 = m_Value1.GetVariant(context);
			if (Val1.IsEmpty)
			{
				return m_Value2.GetString(context);
			}
			return m_Value1.GetString(context);
		}
		override internal Variant GetVariant(CallingContext context)
		{
			Variant Val1 = m_Value1.GetVariant(context);
			if (Val1.IsEmpty)
			{
				Val1 = new Variant(m_Value2.GetVariant(context));
			}
			return Val1;
		}
	}

}