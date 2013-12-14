using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class IIF : ExpressionBase
	{
		ExpressionBase m_bCondition;
		ExpressionBase m_Value1;
		ExpressionBase m_Value2;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
				case 2:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 3:
					m_bCondition = param[0];
					m_Value1 = param[1];
					m_Value2 = param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			
			// dunno how it is important to be known here
			// cant be decided here or by calling GetVarType(comp.Context)
		}

		override internal string GetString(CallingContext context)
		{
			bool Cond = m_bCondition.GetBool(context);
			if(Cond)
			{
				return m_Value1.GetString(context);
			}
			
			return m_Value2.GetString(context);
		}

		override internal Variant GetVariant(CallingContext context)
		{
			bool Cond = m_bCondition.GetBool(context);
			if (Cond)
			{
				return new Variant(m_Value1.GetVariant(context));
			}

			return new Variant(m_Value2.GetVariant(context));
		}

		internal override VariantType GetVarType(CallingContext context)
		{
			return GetVariant(context).Type;
		}
	}

}