using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu
{
	class ALEN : ExpressionBase
	{
		ExpressionBase array;
		ExpressionBase flags;

		// the rest of the params are a paramlist ...
		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();

			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					array = param[0];
					break;
				case 2:
					array = param[0];
					flags = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		internal override int GetInt(CallingContext context)
		{
			ArrayMember arr;
			if (array is ReadArray)
				arr = ((ReadArray)array).GetArray(context);
			else if (array is ArrayMemberAccess)
				arr = ((ArrayMemberAccess)array).GetArray(context);
			else
				arr = null;
			if (arr == null)
				throw new ErrorException(ErrorCodes.NotAnArray, array.GetName(context).ToUpper(System.Globalization.CultureInfo.InvariantCulture));

			Int32 flag;
			if (flags == null)
				flag = 0;
			else
				flag = flags.GetInt(context);

			switch (flag)
			{
				case 0:
					return (Int32) (arr.Dimension1 * arr.Dimension2);
				case 1:
					return (Int32) arr.Dimension1;
				case 2:
					if (arr.Dimensions == 1)
						return 0;

						return (Int32) arr.Dimension2;
				default:
					throw new ErrorException(ErrorCodes.InvalidArgument);
			}
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return new Variant(GetInt(context),10);
		}

	}
}
