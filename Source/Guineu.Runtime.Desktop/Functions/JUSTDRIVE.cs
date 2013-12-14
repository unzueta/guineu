using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu.Functions
{
	class JUSTDRIVE : ExpressionBase
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
			try
			{
				string s = path.GetString(context).Trim();
				if (s.IndexOf(Path.VolumeSeparatorChar) > 0)
				{
					return s.Substring(0, s.IndexOf(Path.VolumeSeparatorChar)+1);
				}
				return string.Empty;
			}
			catch (ArgumentException)
			{
				return "";
			}
		}

		internal override VariantType GetVarType(CallingContext context)
		{
			return VariantType.Character;
		}
	}
}