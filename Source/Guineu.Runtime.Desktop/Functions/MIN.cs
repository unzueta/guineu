using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class MIN : ExpressionBase
	{
		List<ExpressionBase> m_Param;

		override internal void Compile(Compiler comp)
		{
			m_Param = comp.GetParameterList();
			if (m_Param.Count < 2)
				throw new ErrorException(ErrorCodes.TooFewArguments);
		}

		override internal Variant GetVariant(CallingContext context)
		{
			// save the type of param[0]
			Variant min = m_Param[0].GetVariant(context);
			VariantType vt = min.Type;
			Variant curValue;

			if (min.IsNull)
				return new Variant(vt, true);

			foreach (ExpressionBase exp in m_Param)
			{
				curValue = exp.GetVariant(context);
				if(curValue.Type!=vt)
					throw new ErrorException(ErrorCodes.InvalidArgument);
				if (curValue.IsNull)
					return new Variant(vt, true);

				if (curValue.IsLessThan(min))
					min = curValue;
			}
			return new Variant(min);
		}

	}

}