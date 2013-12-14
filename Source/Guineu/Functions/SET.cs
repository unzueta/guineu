using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SetFunction : ExpressionBase
	{
		ExpressionBase function;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				case 1:
					function = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Nti name = function.ToNti(context);
			switch (name.ToKnownNti())
			{
				case KnownNti.Poin:
				case KnownNti.Point:
					return new Variant(GuineuInstance.Set.Point.Value);
			}
			throw new ErrorException(ErrorCodes.InvalidSetArgument);
		}
	}
}