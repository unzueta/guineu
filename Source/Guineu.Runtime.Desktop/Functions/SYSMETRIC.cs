using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class SYSMETRIC : ExpressionBase
	{
		ExpressionBase value;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					value = param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetInt(context), 11);
			return retVal;
		}

		internal override int GetInt(CallingContext context)
		{
			Int32 option = value.GetInt(context);
			switch (option)
			{
				case 1:
					return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
				case 2:
					return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
				default:
					throw new ErrorException(ErrorCodes.InvalidArgument);
			}
		}
	}

}