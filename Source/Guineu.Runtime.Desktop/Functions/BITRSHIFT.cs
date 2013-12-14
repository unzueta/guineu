using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class BITRSHIFT : ExpressionBase
	{	// TODO : varbinary and blob
		ExpressionBase m_nNumExp1;
		ExpressionBase m_nNumExp2;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();

			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					m_nNumExp1 = param[0];
					m_nNumExp2 = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}

			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			return new Variant(GetInt(context), 15);
		}

		internal override int GetInt(CallingContext context)
		{
			int retVal = 0;
			int val1, val2;
			val1 = m_nNumExp1.GetInt(context); // the value to reset the bit
			val2 = m_nNumExp2.GetInt(context); // the bit number to reset

			retVal = val1 >> val2;

			return retVal;
		}
	}
}