using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class LOWER : ExpressionBase
	{
		ExpressionBase value;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					value = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (value.CheckString(context, false))
				return new Variant(VariantType.Character, true);

			return new Variant(GetUpper(context));
		}

		internal override string GetString(CallingContext context)
		{
			return GetUpper(context);
		}

		string GetUpper(CallingContext context)
		{
			return value.GetString(context).ToLower(System.Globalization.CultureInfo.InvariantCulture);
		}

	}

}