using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SPACE : ExpressionBase
	{
		ExpressionBase count;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				default:
					count = param[0];
					break;
			}
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			int width = count.GetInt(exec);
			if (width > 0)
				return new Variant("".PadRight(width));
			
			return new Variant("");
		}

		internal override string GetString(CallingContext exec)
		{
			int width = count.GetInt(exec);
			return "".PadRight(width);
		}
	}
}