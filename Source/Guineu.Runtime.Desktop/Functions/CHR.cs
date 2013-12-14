using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class CHR : ExpressionBase
	{
		ExpressionBase m_ASC;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					m_ASC = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}


		override internal Variant GetVariant(CallingContext context)
		{
			return new Variant(GetString(context));
		}

		internal override string GetString(CallingContext context)
		{
			byte[] code = new byte[1];
			code[0] = (byte) m_ASC.GetInt(context);
			// (...) CODEPAGE= setting. AUTO=Encoding.Default, otherwise our current code page 
			string chr = GuineuInstance.CurrentCp.GetString(code,0,code.Length);
			return chr;
		}
	}

}