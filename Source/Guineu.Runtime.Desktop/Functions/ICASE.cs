using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class ICASE : ExpressionBase
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
			int i = 0;
			bool cond = false;
			int rest = 0;
			rest = m_Param.Count % 2;
			bool hasOtherWise = (rest != 0);
			int lastCond = hasOtherWise ? m_Param.Count - 2 : m_Param.Count - 1;
			while (i < lastCond)
			{
				cond = m_Param[i].GetBool(context);
				i++;
				if (cond)
					return m_Param[i].GetVariant(context);

				i++;
			}
			// if otherwise return it else return NULL
			if (hasOtherWise)
			{
				return m_Param[m_Param.Count - 1].GetVariant(context);
			}

			return new Variant(VariantType.Null, true);
		}

	}
}