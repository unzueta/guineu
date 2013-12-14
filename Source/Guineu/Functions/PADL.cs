using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class PADL : ExpressionBase
	{
		ExpressionBase expression;
		ExpressionBase size;
		ExpressionBase fillchar;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					expression = param[0];
					size = param[1];
					break;
				case 3:
					expression = param[0];
					size = param[1];
					fillchar = param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (fillchar != null)
			{
				if (fillchar.CheckString(context, true))
					return new Variant(VariantType.Character, true);
			}

			string cExp = expression.GetString(context);
			int nSize = size.GetInt(context);
			int remain = nSize - cExp.Length;

			if (remain > 0)
			{
				string cChar;
				if (fillchar != null)
					cChar = fillchar.GetString(context);
				else
					cChar = " ";

				while (cChar.Length < remain)
					cChar += cChar;
				cChar = cChar.Substring(0, remain);
				cExp = cChar + cExp;
			}

			var ret=cExp.Substring(0, nSize);

			return new Variant(ret);
		}
	}
}
