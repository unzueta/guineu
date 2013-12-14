using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu
{
	class HOUR : ExpressionBase
	{
		ExpressionBase dateTimeValue;

		override internal void Compile(Compiler comp)
		{
			// Get all parameters
			List<ExpressionBase> param = comp.GetParameterList();

			// ALLTRIM() has been called without any parameters
			if (param.Count == 0)
			{
				throw new ErrorException(ErrorCodes.TooFewArguments);
			}

			// ALLTRIM() has been called with more than one parameter
			if (param.Count > 1)
			{
				throw new ErrorException(ErrorCodes.TooManyArguments);
			}

			dateTimeValue = param[0];
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			return new Variant(GetInt(exec), 1);
		}

		internal override double GetDouble(CallingContext exec)
		{
			return 1.0 * GetInt(exec);
		}

		internal override int GetInt(CallingContext exec)
		{
			Variant value = dateTimeValue.GetVariant(exec);

			// Only dates and datetimes are valid parameters
			if (value.Type == VariantType.Date)
				return 0;

			if (value.Type != VariantType.DateTime)
			{
				throw new ErrorException(ErrorCodes.InvalidArgument);
			}

			DateTime date = value;

			if (date.Ticks == 0)
				return 0;

			return date.Hour;
		}
	}
}
