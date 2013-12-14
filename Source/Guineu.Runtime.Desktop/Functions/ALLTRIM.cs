using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class ALLTRIM : ExpressionBase
	{
		ExpressionBase value;

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

			value = param[0];
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (value.CheckString(context, true))
				return new Variant(VariantType.Character, true);

			return new Variant(value.GetString(context).Trim());
		}

		internal override string GetString(CallingContext exec)
		{
			string s = value.GetString(exec);
			if (s != null)
			{
				s = s.Trim();
			}
			return s;
		}

	}

}