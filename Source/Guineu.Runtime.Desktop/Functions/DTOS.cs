using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class DTOS : ExpressionBase
	{
		ExpressionBase dateExpression;

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
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			Variant value = dateExpression.GetVariant(exec);
			if (value.IsNull)
				return new Variant(VariantType.Character, true);

			// Only dates and datetimes are valid parameters
			if (value.Type != VariantType.Date && value.Type != VariantType.DateTime)
				throw new ErrorException(ErrorCodes.InvalidArgument);

			string retVal = string.Empty;
			value = dateExpression.GetVariant(exec);

			if (value.Type == VariantType.DateTime || value.Type == VariantType.Date)
				retVal = ((DateTime)value).ToString("yyyyMMdd");

			return new Variant(retVal);
		}
	}
}
