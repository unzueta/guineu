using System;
using System.Collections.Generic;
using Guineu.Expression;
using Guineu.Core;

namespace Guineu
{
	partial class AERROR : ExpressionBase
	{
		WriteArray array;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					array = (WriteArray)param[0];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		private static void ChangeArraySize(int numRows, int numCols, ArrayMember arr)
		{
			arr.Dimension(numRows, numCols);
		}

		override internal Variant GetVariant(CallingContext context)
		{
			ArrayMember arr = array.GetArray(context);
			if (arr == null)
			{
				throw new ErrorException(ErrorCodes.NotAnArray, array.GetName(context).ToUpper(System.Globalization.CultureInfo.InvariantCulture));
			}

			ChangeArraySize(GuineuInstance.Errors.Count, 7, arr);

			Int32 item = 0;
			foreach (ErrorItemBase error in GuineuInstance.Errors)
			{
				item++;
				error.FillErrorArray(arr, item);
			}
			return new Variant(GuineuInstance.Errors.Count, 10);
		}
	}
}