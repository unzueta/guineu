using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class BITCLEAR : ExpressionBase
	{	// TODO : varbinary and blob
		ExpressionBase m_nNumExp1;
		ExpressionBase m_nNumExp2;
		ExpressionBase m_nStartBit;
		ExpressionBase m_nBitCount;

		int m_ParamCount;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			m_ParamCount = param.Count;

			switch (m_ParamCount)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					m_nNumExp1 = param[0];
					break;
				case 2:
					m_nNumExp1 = param[0];
					m_nNumExp2 = param[1];
					break;
				case 3:
					m_nNumExp1 = param[0];
					m_nStartBit = param[1];
					m_nBitCount = param[2];
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
			if (m_ParamCount == 2)
			{
				int val1, val2;
				val1 = m_nNumExp1.GetInt(context); // the value to reset the bit
				val2 = m_nNumExp2.GetInt(context); // the bit number to reset
				try
				{
					if (val2 >= 0 && val2 <= 31)
					{
						val2 = (int)Math.Pow(2, val2);
						if ((val1 & val2) != 0)
							retVal = val1 ^ val2;
					}
					else
						retVal = val1;
				}
				catch (Exception)
				{
					throw new ErrorException(ErrorCodes.InvalidSubscript);
				}
			}
			else
			{
				int val1, start, count;
				val1 = m_nNumExp1.GetInt(context);	// the value to reset bits in
				retVal = val1;

				if (m_ParamCount == 1)
				{
					start = 0;
					count = 31;
				}
				else
				{
					start = m_nStartBit.GetInt(context);// the index
					count = m_nBitCount.GetInt(context);// the count
					if (start < 0 || start > 31)
						start = 0;
					if (count < 0 || count + start > 31)
						count = 31 - start;
				}
				try
				{
					for (int i = start; i <= count; i++)
					{
						if ((((int)Math.Pow(2, i)) & val1) != 0)
							retVal = retVal ^ (int)Math.Pow(2, i);
					}
				}
				catch (Exception)
				{
					throw new ErrorException(ErrorCodes.InvalidSubscript);
				}
			}

			return retVal;
		}
	}
}