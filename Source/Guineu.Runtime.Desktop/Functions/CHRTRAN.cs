using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class CHRTRAN : ExpressionBase
	{
		ExpressionBase expression;
		ExpressionBase searched;
		ExpressionBase replacement;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();

			switch (param.Count)
			{
				case 0:
				case 1:
				case 2:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 3:
					expression = param[0];
					searched = param[1];
					replacement = param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (expression.CheckString(context, false))
				return new Variant(VariantType.Character, true);

			if (searched.CheckString(context, false))
				return new Variant(VariantType.Character, true);

			if (replacement.CheckString(context, false))
				return new Variant(VariantType.Character, true);

			return new Variant(GetString(context));
		}

		internal override string GetString(CallingContext context)
		{
			if (expression.CheckString(context, false))
				return null;

			if (searched.CheckString(context, false))
				return null;

			if (replacement.CheckString(context, false))
				return null;

			return GetChrTranString(context);
		}

		private string GetChrTranString(CallingContext context)
		{
			String expr = expression.GetString(context);
			String search = searched.GetString(context);
			String replace = replacement.GetString(context);

			if (String.IsNullOrEmpty(expr))
				return expr;
			if (String.IsNullOrEmpty(search))
				return expr;

			Int32 loopMax = Math.Min(search.Length, replace.Length);
			Int32 loop;

			for (loop = 0; loop < loopMax; loop++)
			{
				char[] oldChar = search.Substring(loop, 1).ToCharArray();
				char[] newChar = replace.Substring(loop, 1).ToCharArray();
				expr = expr.Replace(oldChar[0], newChar[0]);
			}

			if (search.Length > replace.Length)
			{
				int pos = 0;
				while (loop < search.Length)
				{
					// remove the other
					char[] c = search.Substring(loop, 1).ToCharArray();
					while (pos >= 0)
					{
						pos = expr.IndexOf(c[0]);
						if (pos >= 0)
							expr = expr.Remove(pos, 1);
					}
					loop++;
				}
			}

			return expr;
		}
	}
}
