using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu.Functions
{
	class ADDBS : ExpressionBase
	{
		ExpressionBase path;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					path = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant value = path.GetVariant(context);
			if (value.Type != VariantType.Character)
			{
				throw new ErrorException(ErrorCodes.InvalidArgument);
			}
			return new Variant(GetString(context));
		}

		internal override string GetString(CallingContext context)
		{
			String s = path.GetString(context);
			if( s.Length == 0 || s[s.Length-1] != Path.DirectorySeparatorChar )
			{
				s = s + Path.DirectorySeparatorChar;
			}
			return s;
		}

	}

}