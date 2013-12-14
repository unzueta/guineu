using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SUBSTR : ExpressionBase
	{
		ExpressionBase value;
		ExpressionBase position;
		ExpressionBase length;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					value = param[0];
					position = param[1];
					break;
				case 3:
					value = param[0];
					position = param[1];
					length = param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (value.CheckString(context, false))
				return new Variant(VariantType.Character, true);
			return new Variant(GetSubString(context, value.GetString(context)));
		}

		internal override string GetString(CallingContext context)
		{
			return GetSubString(context, value.GetString(context));
		}

		string GetSubString(CallingContext context, string val)
		{
			int pos = position.GetInt(context);
			string retString;
			if (length == null)
			{
				retString = val.Substring(pos - 1);
			}
			else
			{
				int len = length.GetInt(context);
				var maxLen = val.Length - pos + 1;
				retString = val.Substring(pos - 1, len > maxLen ? maxLen : len);
			}
			return retString;
		}

	}

}