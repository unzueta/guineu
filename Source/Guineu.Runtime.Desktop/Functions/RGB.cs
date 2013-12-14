using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class RGB : ExpressionBase
	{
		ExpressionBase _Red;
		ExpressionBase _Green;
		ExpressionBase _Blue;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
				case 1:
				case 2:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 3:
					_Red = param[0];
					_Green = param[1];
					_Blue = param[2];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			Variant retVal = new Variant(GetInt(context), 10);
			return retVal;
		}

		internal override int GetInt(CallingContext context)
		{
			return 
				_Red.GetInt(context) + 
				256 * _Green.GetInt(context) + 
				65536 * _Blue.GetInt(context);
		}

	}

}