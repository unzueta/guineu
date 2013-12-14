using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu
{
	class YEAR : ExpressionBase
	{
		ExpressionBase date;

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

			date = param[0];
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			return new Variant(GetInt(exec), 5);
		}

		internal override double GetDouble(CallingContext exec)
		{
			return GetInt(exec);
		}

		internal override int GetInt(CallingContext exec)
		{
			Variant value = date.GetVariant(exec);

			// Only dates and datetimes are valid parameters
			if (value.Type != VariantType.Date && value.Type != VariantType.DateTime)
			{
				throw new ErrorException(ErrorCodes.InvalidArgument);
			}

			DateTime dateValue = value;

			if (dateValue.Ticks == 0)
				return 0;

			return dateValue.Year;
		}
	}
}
