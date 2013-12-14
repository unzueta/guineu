using Guineu.Expression;

namespace Guineu.Functions
{
	class TTOD : ExpressionBase
	{
		ExpressionBase dateTime;

	override internal void Compile(Compiler comp)
		{
			var param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					dateTime = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var dt = dateTime.GetVariant(context);
			return new Variant(dt, VariantType.Date);
		}
	}
}
