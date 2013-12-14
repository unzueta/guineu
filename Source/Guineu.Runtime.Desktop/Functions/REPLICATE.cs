using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu.Functions
{
	class REPLICATE : ExpressionBase
	{
		ExpressionBase count;
		ExpressionBase str;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				default:
					count = param[1];
					str = param[0];
					break;
			}
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			if (str.CheckString(exec, false))
				return new Variant(VariantType.Character, true);

			// Check parameter #2
			Variant value = count.GetVariant(exec);
			if (value.Type != VariantType.Integer)
			{
				throw new ErrorException(ErrorCodes.InvalidArgument);
			}

			return new Variant(GetString(exec));
		}

		internal override string GetString(CallingContext exec)
		{
			int times = count.GetInt(exec);
			string expression = str.GetString(exec);
			return Replicate( expression, times);
		}

		private static string Replicate(string cExpression, int nTimes)
		{
			//Create a stringBuilder
			var sb = new StringBuilder();

			//Insert the expression into the StringBuilder for nTimes
			sb.Insert(0, cExpression, nTimes);

			//Convert it to a string and return it back
			return sb.ToString();
		}
        

	}
}