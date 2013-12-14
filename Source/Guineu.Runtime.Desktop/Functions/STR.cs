using System;
using Guineu.Expression;

namespace Guineu.Functions
{
	class STR : FunctionBase
	{
		ExpressionBase number;
		ExpressionBase length;
		ExpressionBase decimals;

		override internal void Compile(Compiler comp)
		{
			GetParameters(comp, 1, 2, 3);
			number = GetParam(0);
			length = GetParam(1);
			decimals = GetParam(2);
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			Variant value = number.GetVariant(exec);
			if (value.IsNull)
				return new Variant(VariantType.Character, true);

			if (value.Type != VariantType.Number && value.Type != VariantType.Integer)
				throw new ErrorException(ErrorCodes.InvalidArgument);

			Int32 l = length == null ? 10 : length.GetInt(exec);
			Int32 dp = decimals == null ? 0 : decimals.GetInt(exec);

			String fs = "{0," + l + ":0." + new String('0', dp) + "}";
			String s = String.Format(fs, (double) value);
			return new Variant(s);
		}

		internal override string GetString(CallingContext exec)
		{
			string value = number.GetString(exec);
			return value;
		}
	}
}