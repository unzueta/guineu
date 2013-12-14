using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class ASC : ExpressionBase
	{
		ExpressionBase m_Char;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					m_Char = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		
		override internal Variant GetVariant(CallingContext context)
		{
			return new Variant(GetInt(context), 3);
		}

		internal override int GetInt(CallingContext context)
		{
			string chr = m_Char.GetString(context);
			int retVal;
			if (chr.Length == 0)
			{
				retVal = 0;
			}
			else
			{
				retVal = (Encoding.Default.GetBytes(chr.Substring(0, 1)))[0];
			}
			return retVal;
		}
	}

}