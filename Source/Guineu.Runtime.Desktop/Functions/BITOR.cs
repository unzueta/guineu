using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class BITOR : ExpressionBase
	{	// TODO : varbinary and blob
		List<ExpressionBase> m_Params;
		int m_ParamCount;

		override internal void Compile(Compiler comp)
		{
			m_Params = comp.GetParameterList();
			m_ParamCount = m_Params.Count;

			if (m_ParamCount < 2)
			{
				throw new ErrorException(ErrorCodes.TooFewArguments);
			}

			FixedInt = true;

		}

		override internal Variant GetVariant(CallingContext context)
		{
			foreach (ExpressionBase value in m_Params)
			{
				if (value.GetVariant(context).IsNull)
					return new Variant(VariantType.Character, true);
			}

			return new Variant(GetInt(context), 15);
		}

		internal override int GetInt(CallingContext context)
		{
			int retVal = m_Params[0].GetInt(context);

			for(int i = 1; i<m_Params.Count; i++)
			{
				retVal = retVal | m_Params[i].GetInt(context);
			}

			return retVal;
		}

		internal override double GetDouble(CallingContext context)
		{
			return 1.0 * GetInt(context);
		}
	}

}