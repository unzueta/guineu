using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu.Functions
{
	class STUFF : ExpressionBase
	{
		ExpressionBase expression;
		ExpressionBase startReplacement;
		ExpressionBase charReplaced;
		ExpressionBase replacement;

		int paramCount;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			paramCount = param.Count;

			switch (paramCount)
			{
				case 0:
				case 1:
				case 2:
				case 3:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 4:
					expression = param[0];
					startReplacement = param[1];
					charReplaced = param[2];
					replacement = param[3];
					break;

				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}


		override internal Variant GetVariant(CallingContext context)
		{
			if (expression.CheckString(context, false))
				return new Variant(VariantType.Character, true);

			if (replacement.CheckString(context, false))
				return new Variant(VariantType.Character, true);

			return new Variant(GetString(context));
		}

		internal override string GetString(CallingContext context)
		{
			if (expression.CheckString(context, false))
				return null;

			if (replacement.CheckString(context, false))
				return null;

			return GetSubString(context);
		}

		internal string GetSubString(CallingContext context)
		{
			string cExpression = expression.GetString(context);
			int nStartReplacement = startReplacement.GetInt(context);
			int nCharactersReplaced = charReplaced.GetInt(context);
			string cReplacement = replacement.GetString(context);
			var sb = new StringBuilder(cExpression);

			if (nStartReplacement < 1)
				nStartReplacement = 1;
			if (nCharactersReplaced < 0)
				nCharactersReplaced = 0;

			if (nCharactersReplaced + nStartReplacement - 1 >= cExpression.Length)
				nCharactersReplaced = cExpression.Length - nStartReplacement + 1;

			if (nCharactersReplaced != 0)
			{
				sb.Remove(nStartReplacement - 1, nCharactersReplaced);
			}

			sb.Insert(nStartReplacement - 1, cReplacement);
			return sb.ToString();
		}
	}
}



