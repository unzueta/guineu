using System;
using System.Collections.Generic;
using System.Globalization;
using Guineu.Expression;

namespace Guineu.Functions
{
	class TRANSFORM : ExpressionBase
	{
		ExpressionBase valueExpression;
		ExpressionBase formatExpression;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					valueExpression = param[0];
					break;
				case 2:
					valueExpression = param[0];
					formatExpression = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var retVal = new Variant(GetString(context));
			return retVal;
		}

		internal override string GetString(CallingContext context)
		{
			string retVal;
			if (formatExpression == null)
			{
				retVal = valueExpression.GetString(context);
			}
			else if (valueExpression.FixedInt)
			{
				retVal = TransformInt(context);
			}
			else
			{
				retVal = TransformVariant(context);
			}
			return retVal;
		}

		string TransformInt(CallingContext context)
		{
			string format = formatExpression.GetString(context);
			string normalizedFormat = format.Trim();
			int value = valueExpression.GetInt(context);
			string retVal;

			// @0 cannot be combined with other codes.
				if (String.Compare(normalizedFormat, "@0", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				retVal = "0x" + value.ToString("X8",NumberFormatInfo.CurrentInfo);
			}
			else
			{
				// (other formats)
				retVal = valueExpression.GetString(context);
			}
			return retVal;
		}

		string TransformVariant(CallingContext context)
		{
			string format = formatExpression.GetString(context);
			string normalizedFormat = format.Trim();
			Variant value = valueExpression.GetVariant(context);
			string retVal;

			// @0 cannot be combined with other codes.
			if (String.Compare(normalizedFormat, "@0", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				retVal = "0x" + ((Int32)value).ToString("X8", NumberFormatInfo.CurrentInfo);
			}
			else
			{
				// (other formats)
				retVal = valueExpression.GetString(context);
			}
			return retVal;
		}


	}

}