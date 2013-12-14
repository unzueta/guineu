using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class TIME : ExpressionBase
	{

		override internal void Compile(Compiler comp)
		{
				List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			return new Variant(GetString(exec));
		}

		internal override string GetString(CallingContext exec)
		{
			// (...) Evaluate the current setting of SET HOUR TO
			string retVal = DateTime.Now.ToString("HH:mm:ss");
			return retVal;
		}
	}
}
