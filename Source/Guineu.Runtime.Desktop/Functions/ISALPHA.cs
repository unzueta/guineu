using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class ISALPHA : ExpressionBase
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

			if (val.Type != VariantType.Character)
				throw new ErrorException(ErrorCodes.InvalidArgument);

			String str = val;
			if (str.Length == 0)
				return false;
			return Char.IsLetter(str, 0);
		}
	}

}