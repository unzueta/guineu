using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	class STRTRAN : ExpressionBase
	{
		ExpressionBase searchExpression;
		ExpressionBase searchedExpression;
		ExpressionBase replacementExpression;
		ExpressionBase startExpression;
		ExpressionBase numberExpression;
		ExpressionBase flagExpression;

		int paramCount;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			paramCount = param.Count;

			switch (paramCount)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					searchExpression = param[0];
					searchedExpression = param[1];
					break;
				case 3:
					searchExpression = param[0];
					searchedExpression = param[1];
					replacementExpression = param[2];
					break;
				case 4:
					searchExpression = param[0];
					searchedExpression = param[1];
					replacementExpression = param[2];
					startExpression = param[3];
					break;
				case 5:
					searchExpression = param[0];
					searchedExpression = param[1];
					replacementExpression = param[2];
					startExpression = param[3];
					numberExpression = param[4];
					break;
				case 6:
					searchExpression = param[0];
					searchedExpression = param[1];
					replacementExpression = param[2];
					startExpression = param[3];
					numberExpression = param[4];
					flagExpression = param[5];
					break;

				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		private bool CheckParam(CallingContext context)
		{
			Variant value;

			// Check parameter #4
			if (startExpression != null)
			{
				value = startExpression.GetVariant(context);
				if (value.Type != VariantType.Integer && value.Type != VariantType.Number)
					throw new ErrorException(ErrorCodes.InvalidArgument);
				int nOcc = value;
				if (nOcc < -1 || nOcc == 0)
					throw new ErrorException(ErrorCodes.InvalidArgument);
			}

			// Check parameter #5
			if (numberExpression != null)
			{
				value = numberExpression.GetVariant(context);
				if (value.Type != VariantType.Integer && value.Type != VariantType.Number)
					throw new ErrorException(ErrorCodes.InvalidArgument);
				int nOcc = value;
				if (nOcc < -1 || nOcc == 0)
					throw new ErrorException(ErrorCodes.InvalidArgument);
			}

			// Check parameter #6
			if (flagExpression != null)
			{
				value = flagExpression.GetVariant(context);
				if (value.IsNull)
				{
					throw new ErrorException(ErrorCodes.InvalidArgument);
				}
				// Only int is a valid parameter
				if (value.Type != VariantType.Integer && value.Type != VariantType.Number)
				{
					throw new ErrorException(ErrorCodes.InvalidArgument);
				}

				// the flag can't be greater than 7 (range : 0 to 7)
				int nOcc = value;

				if (nOcc < -1 || nOcc > 3)
				{
					throw new ErrorException(ErrorCodes.InvalidArgument);
				}
			}

			return true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			if (searchedExpression.CheckString(context, false))
				return new Variant(VariantType.Character, true);

			if (searchExpression.CheckString(context, false))
				return new Variant(VariantType.Character, true);

			if (replacementExpression.CheckString(context, false))
				return new Variant(VariantType.Character, true);

			return new Variant(GetString(context));
		}

		internal override string GetString(CallingContext context)
		{
			if (searchedExpression.CheckString(context, false))
				return null;

			if (searchExpression.CheckString(context, false))
				return null;

			if (replacementExpression.CheckString(context, false))
				return null;

			// if one of the 3 first params is NULL, return NULL
			if (CheckParam(context))
				return GetStrTranString(context);
			return null;
		}

		private string GetStrTranString(CallingContext context)
		{
			switch (paramCount)
			{
				case 2:
					// call the sub routine with default 3,4,5 params...
					return StrTran(context, searchedExpression.GetString(context), searchExpression.GetString(context), "", 1, -1, 1);
				case 3:
					// call the sub routine with default  4,5 params...
					return StrTran(context, searchedExpression.GetString(context), searchExpression.GetString(context), replacementExpression.GetString(context), 1, -1, 1);
				case 4:
					// call the sub routine with default 5 param...
					return StrTran(context, searchedExpression.GetString(context), searchExpression.GetString(context), replacementExpression.GetString(context), startExpression.GetInt(context), -1, 1);
				case 5:
					// call the sub routine ...
					return StrTran(context, searchedExpression.GetString(context), searchExpression.GetString(context), replacementExpression.GetString(context), startExpression.GetInt(context), numberExpression.GetInt(context), 1);
				case 6:
					// call the sub routine ...
					return StrTran(context, searchedExpression.GetString(context), searchExpression.GetString(context), replacementExpression.GetString(context), startExpression.GetInt(context), numberExpression.GetInt(context), flagExpression.GetInt(context));
			}
			// shouldn't happen
			return null;
		}

		private static int At(string cSearchFor, string cSearchIn, int occurence)
		{
			int count = occurence <= 0 ? 1 : occurence;

			string value = cSearchIn;
			string search = cSearchFor;

			Int32 retVal = 0;
			for (Int32 loop = 0; loop < count; loop++)
			{
				retVal = value.IndexOf(search, retVal, value.Length - retVal, StringComparison.InvariantCulture);
				retVal = retVal + 1;
				if (retVal == 0)
				{
					break;
				}
			}
			return retVal;
		}

		public static string StrTran(CallingContext context, string cSearchFor, string cSearchIn, string cReplaceWith, int nStartoccurence, int nCount, int nFlags)
		{
			string lcRetVal = string.Empty;
			int count = 0;

			string searchForUpper = cSearchFor;
			searchForUpper.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
			string searchInUpper;

			bool bSensitive = nFlags == 0 || nFlags == 2;
			bool bExactReplace = nFlags == 1;

			if (nStartoccurence < 1)
				nStartoccurence = 1;

			// do nothing case

			while (count < nCount || nCount < 0)
			{
				searchInUpper = cSearchIn;
				searchInUpper.ToUpper(System.Globalization.CultureInfo.InvariantCulture);

				bool bFoundUpper = false;

				int pos;
				if (count == 0)
				{
					if (!bSensitive)
					{
						pos = At(cSearchFor, cSearchIn, nStartoccurence);
						if (pos == 0)
						{
							pos = At(searchForUpper, searchInUpper, nStartoccurence);
							if (pos > 0)
								bFoundUpper = bExactReplace;
						}
					}
					else
						pos = At(cSearchFor, cSearchIn, nStartoccurence);
				}
				else
				{
					if (!bSensitive)
					{
						pos = At(cSearchFor, cSearchIn, 1);
						if (pos == 0)
						{
							pos = At(searchForUpper, searchInUpper, 1);
							if (pos > 0)
								bFoundUpper = bExactReplace;
						}
					}
					else
						pos = At(cSearchFor, cSearchIn, 1);
				}

				if (pos > 0)
				{
					// walk thru the string and do the stuff ...
					lcRetVal += cSearchIn.Substring(0, pos - 1);
					if (cReplaceWith != string.Empty)
					{
						if (bFoundUpper)
							lcRetVal += cReplaceWith.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
						else
							lcRetVal += cReplaceWith;
					}
					cSearchIn = cSearchIn.Substring(pos - 1 + cSearchFor.Length);
					count++;
				}
				else if (pos == 0)
				{
					return lcRetVal + cSearchIn;
				}
			}

			return lcRetVal;
		}
	}
}
