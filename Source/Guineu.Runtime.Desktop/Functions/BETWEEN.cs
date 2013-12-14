using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class BETWEEN : ExpressionBase
	{
		ExpressionBase m_Value;
		ExpressionBase m_Min;
		ExpressionBase m_Max;

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
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 3:
					m_Value = param[0];
					m_Min = param[1];
					m_Max = param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (m_Value.GetVariant(context).IsNull)
			{
				return new Variant(VariantType.Logical,true);
			}
			if (m_Min.GetVariant(context).IsNull)
			{
				return new Variant(VariantType.Logical,true);
			}
			if (m_Max.GetVariant(context).IsNull)
			{
				return new Variant(VariantType.Logical,true);
			}

			return new Variant(GetBool(context));
		}

		internal override bool GetBool(CallingContext context)
		{
			Variant val = m_Value.GetVariant(context);
			Variant min = m_Min.GetVariant(context);
			Variant max = m_Max.GetVariant(context);

			if( val.IsLessThan(min) )
			{
				return false;
			}
			if (val.IsGreaterThan(max))
			{
				return false;
			}
			return true;
		}
	}

}