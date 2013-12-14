using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class FILE : ExpressionBase
	{
		ExpressionBase fileName;

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

			fileName = param[0];
		}

		internal override bool GetBool(CallingContext context)
		{
			// Only strings are valid parameters
			Variant value = fileName.GetVariant(context);
			if (value.Type != VariantType.Character)
			{
				throw new ErrorException(ErrorCodes.InvalidArgument);
			}

			// Otherwise we trim the string
			Boolean fileExists = GuineuInstance.FileMgr.Exists(fileName.GetString(context));
			return fileExists;
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return new Variant(GetBool(context));
		}
	}

}