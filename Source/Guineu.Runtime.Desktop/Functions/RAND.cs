using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class RAND : ExpressionBase
	{
		static Random rnd;
		ExpressionBase seedValue;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				case 1:
					seedValue = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetDouble(context), 20, 10);
			return retVal;
		}

		internal override double GetDouble(CallingContext context)
		{
			int nSeed;
			if (seedValue != null)
				nSeed = seedValue.GetInt(context);
			else
				nSeed = 100;

			if(rnd==null)
				rnd = new Random(nSeed);
			
			return rnd.NextDouble();
		}
	}

}
