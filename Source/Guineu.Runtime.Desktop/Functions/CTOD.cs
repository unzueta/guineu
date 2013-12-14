using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class CTOD : ExpressionBase
	{
		ExpressionBase date;
	
		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					date = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			String passedDate = date.GetString(exec);
			DateTime dt;
			try
			{
				dt = DateTime.Parse(passedDate);
			}
			catch
			{
				dt = new DateTime(0);
			}
			return new Variant(dt,VariantType.Date);
		}
	}
}
