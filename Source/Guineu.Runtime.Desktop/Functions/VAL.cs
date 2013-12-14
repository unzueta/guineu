using System;
using System.Collections.Generic;
using System.Globalization;
using Guineu.Expression;

namespace Guineu.Functions
{
	class VAL : ExpressionBase
	{
		ExpressionBase str;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					str = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			Variant value = str.GetVariant(exec);
			if (value.IsNull)
				return new Variant(VariantType.Number, true);

			switch (value.Type)
			{
				case VariantType.Character:
					return new Variant(GetDouble(exec), 20, 10);
				default:
					throw new ErrorException(ErrorCodes.InvalidArgument);
			}
		}

		internal override double GetDouble(CallingContext exec)
		{
			var s = str.GetString(exec);

			double d;
			try
			{
				d = Double.Parse(s, GuineuInstance.Set.CurrentCulture);
			}
			catch (Exception)
			{
				d = 0.00; // TODO: Return number before non-numeric characters instead.
			}
			return d;
		}
	}
}