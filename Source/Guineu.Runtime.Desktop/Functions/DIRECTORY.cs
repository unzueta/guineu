using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu.Functions
{
	class DIRECTORY : ExpressionBase
	{
		ExpressionBase path;

		override internal void Compile(Compiler comp)
		{
			// Get all parameters
			List<ExpressionBase> param = comp.GetParameterList();

			// FILE() has been called without any parameters
			if (param.Count == 0)
			{
				throw new ErrorException(ErrorCodes.TooFewArguments);
			}

			// FILE() has been called with more than one parameter
			if (param.Count > 1)
			{
				throw new ErrorException(ErrorCodes.TooManyArguments);
			}

			path = param[0];
		}

		internal override bool GetBool(CallingContext context)
		{
			// Only strings are valid parameters
			Variant value = path.GetVariant(context);
			if (value.Type != VariantType.Character)
			{
				throw new ErrorException(ErrorCodes.InvalidArgument);
			}

			// Otherwise we trim the string
			try
			{
				Boolean pathExists = Directory.Exists(path.GetString(context).Trim());
				return pathExists;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return new Variant(GetBool(context));
		}
	}

}