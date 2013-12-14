using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class EMPTY : ExpressionBase
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
			return new Variant(GetBool(context));
		}

		internal override bool GetBool(CallingContext context)
		{
			Variant val = value.GetVariant(context);
			if (val.IsNull)
				return false;
			
			switch (val.Type)
			{
				case VariantType.Character:
					if (((String) val).Length == 0)
						return true;
					break;
				case VariantType.Date:
					if (((DateTime) val).Ticks == 0)
						return true;
					break;

				case VariantType.DateTime:
					if (((DateTime)val).Ticks == 0)
						return true;
					break;
				case VariantType.Integer:
					if (val == 0)
						return true;
					break;
				case VariantType.Logical:
					return !val;

				case VariantType.Number:
					return (Double) val == 0;

				default:
					break;
			}

			return false;
		}
	}
}