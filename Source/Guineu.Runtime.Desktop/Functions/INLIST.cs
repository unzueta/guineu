using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class INLIST : ExpressionBase
	{
		List<ExpressionBase> m_Param;
		
		override internal void Compile(Compiler comp)
		{
			m_Param = comp.GetParameterList();
			if(m_Param.Count < 2)
				throw new ErrorException(ErrorCodes.TooFewArguments);
		}

		override internal Variant GetVariant(CallingContext context)
		{	// can't be a getbool as it can return null or maybe there's a way (return a logical false but isnull)
			if (m_Param[0].GetVariant(context).IsNull)
			{
				return new Variant(VariantType.Logical, true);
			}
			VariantType vt = m_Param[0].GetVariant(context).Type;

			int loop=0;
			Variant SearchedValue = m_Param[loop++].GetVariant(context);
			bool hasNULL = false;

			while (loop < m_Param.Count)
			{
				if(m_Param[loop].GetVariant(context).Type!=vt)
					throw new ErrorException(ErrorCodes.InvalidArgument);

				if (SearchedValue.IsEqual(m_Param[loop].GetVariant(context)))
					return new Variant(true);
				if (!hasNULL && m_Param[loop].GetVariant(context).Type == VariantType.Null)
					hasNULL = true;
				loop++;
			}

			if (hasNULL)
				return new Variant(VariantType.Logical, true);
			else
				return new Variant(false);
		}

	}

}