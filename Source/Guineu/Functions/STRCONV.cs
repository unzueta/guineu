using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class STRCONV : ExpressionBase
	{
		ExpressionBase source;
		ExpressionBase conversion;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					source = param[0];
					break;
				case 2:
					source = param[0];
					conversion = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext ctx)
		{
			Int32 destType = conversion.GetInt(ctx);
			switch (destType)
			{
				case 17:
					return StringToUrlEncode(ctx);
				case 18:
					return UrlDecodeFromString(ctx);
			}
			throw new ErrorException(ErrorCodes.InvalidArgument);
		}

		Variant UrlDecodeFromString(CallingContext ctx)
		{
			String s = source.GetString(ctx);
			return new Variant(Uri.UnescapeDataString(s));
		}

		Variant StringToUrlEncode(CallingContext ctx)
		{
			String s = source.GetString(ctx);
			if(String.IsNullOrEmpty(s))
				return new Variant("");
			return new Variant(Uri.EscapeDataString(s));
		}
	}
}