using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class RIGHT : ExpressionBase
	{
		ExpressionBase value;
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
					length = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (value.CheckString(context,false))
				return new Variant(VariantType.Character, true);

			return new Variant(GetSubString(context, value.GetString(context)));
		}

		internal override string GetString(CallingContext context)
		{
			return GetSubString(context, value.GetString(context));
		}

		string GetSubString(CallingContext context, string val)
		{
			int len = length.GetInt(context);
			string retString = val.Substring(val.Length-len);
			return retString;
		}

	}

}