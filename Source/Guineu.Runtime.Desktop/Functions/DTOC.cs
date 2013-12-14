using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class DTOC : ExpressionBase
	{
		ExpressionBase dateExpression;
		ExpressionBase option;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					dateExpression = param[0];
					break;
				case 2:
					dateExpression = param[0];
					option = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			return new Variant(GetString(exec));
		}

		internal override string GetString(CallingContext exec)
		{
			Variant value = dateExpression.GetVariant(exec);
			if (value.IsNull)
			{
				return null;
			}

			// Only dates and datetimes are valid parameters
			if (value.Type != VariantType.Date && value.Type != VariantType.DateTime)
			{
				throw new ErrorException(ErrorCodes.InvalidArgument);
			}

			string retVal = string.Empty;
			value = dateExpression.GetVariant(exec);

			if (value.Type == VariantType.DateTime || value.Type == VariantType.Date)
			{
			    DateTime dt = value;
			    retVal = dt.ToString(option == null ? "dd.MM.yyyy" : "yyyyMMdd");
			}

			return retVal;
		}
	}
}
