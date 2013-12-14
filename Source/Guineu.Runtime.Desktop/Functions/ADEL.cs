using Guineu.Expression;

namespace Guineu.Functions
{
	class ADEL : ExpressionBase
	{
		ReadArray Array;
		ExpressionBase Index;


		override internal void Compile(Compiler comp)
		{
			var param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					Array = (ReadArray) param[0];
					Index = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}
	
		override internal Variant GetVariant(CallingContext context)
		{
			var arr = Array.GetArray(context);
			if (arr == null)
			{
				throw new ErrorException(ErrorCodes.NotAnArray, Array.GetName(context).ToUpper(System.Globalization.CultureInfo.InvariantCulture));
			}

			if(arr.Dimensions == 1)
				arr.ADelElement(Index.GetInt(context));
			else
				arr.ADelRow(Index.GetInt(context));

			return new Variant(1, 10);
		}
	}
}