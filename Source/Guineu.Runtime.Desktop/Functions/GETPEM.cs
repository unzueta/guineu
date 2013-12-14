using System.Collections.Generic;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu
{
	class GETPEM : ExpressionBase
	{
		ExpressionBase obj;
		ExpressionBase property;
		
		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					obj = param[0];
					property = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant objVariant = obj.GetVariant(context);
			ObjectBase reference = objVariant;
			var name = property.ToNti(context);
			var mbr = (ValueMember) reference.GetMember(name);
			Variant retVal = mbr.Get();
			return retVal;
		}
	}

}