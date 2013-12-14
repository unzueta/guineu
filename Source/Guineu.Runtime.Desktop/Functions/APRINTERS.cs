using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu
{
	partial class APRINTERS : ExpressionBase
	{
		ExpressionBase array;
		ExpressionBase flags;

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

		override internal Variant GetVariant(CallingContext context)
		{
			// TODO: This is repeated code. Change to cover all variations.
			ArrayMember arr;
			if (array is WriteArray)
				arr = ((WriteArray)array).GetArray(context);
			else if (array is ArrayMemberAccess)
				arr = ((ArrayMemberAccess)array).GetArray(context);
			else
				arr = null;
			if (arr == null)
				throw new ErrorException(ErrorCodes.NotAnArray, array.GetName(context).ToUpper(System.Globalization.CultureInfo.InvariantCulture));

			// Get the flag parameter
			Int32 flag;
			if (flags == null)
				flag = 0;
			else
				flag = flags.GetInt(context);

			// Depending on the flag value we need to return different values
			switch (flag)
			{
				case 0:
					return GetSimpleInfo(arr, context);
				case 1:
					return GetExtendedInfo(arr, context);
				case 2:
					return GetFastInfo(arr, context);
				default:
					throw new ErrorException(ErrorCodes.InvalidArgument);
			}
		}
	}
}