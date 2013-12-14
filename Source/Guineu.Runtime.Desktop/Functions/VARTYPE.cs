using System;
using System.Collections.Generic;
using System.Text;

using Guineu.Expression;

namespace Guineu
{
	class VARTYPE : ExpressionBase
	{
		ExpressionBase _Expression;
		ExpressionBase _NullDataType;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					_Expression = param[0];
					break;
				case 2:
					_Expression = param[0];
					_NullDataType = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			return new Variant(GetString(context));
		}

		internal override string GetString(CallingContext context)
		{
			VariantType type = _Expression.GetVarType(context);
			if (type == VariantType.Unknown)
				return "U";

			if (_NullDataType == null || !_NullDataType.GetBool(context))
			{
				if (_Expression.IsNull(context))
				{
					return "X";
				}
			}

			switch (type)
			{
				case VariantType.Integer:
					return "I";
				case VariantType.Logical:
					return "L";
				case VariantType.Character:
					return "C";
				case VariantType.Number:
					return "N";
				case VariantType.Object:
					return "O";
				case VariantType.Date:
					return "D";
				case VariantType.DateTime:
					return "T";
				case VariantType.Null:
					return "X";
				default:
					return "U";
			}
		}

	}
}