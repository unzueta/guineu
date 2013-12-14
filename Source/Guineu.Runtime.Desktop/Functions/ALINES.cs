using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu
{
	/// <summary>
	/// ALINES()
	/// </summary>
	partial class ALINES : ExpressionBase
	{
		ExpressionBase array;
		ExpressionBase cExpression;
		ExpressionBase nFlags;
		List<ExpressionBase> param;

		// the rest of the params are a paramlist ...
		override internal void Compile(Compiler comp)
		{
			param = comp.GetParameterList();

			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					array = param[0];
					cExpression = param[1];
					break;
				default:
					array = param[0];
					cExpression = param[1];
					nFlags = param[2];
					break;
			}
			FixedInt = true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			// Get the path

			int nFlag = 1;
			int loop;

			Variant value = cExpression.GetVariant(context);

			if (value.IsNull)
				return new Variant(1, 10);

			string cExpr = cExpression.GetString(context);

			if (nFlags == null)
			{
				nFlag = 0;
				param.RemoveRange(0, param.Count);
			}
			else
			{
				value = nFlags.GetVariant(context);
				int nFirstParser;
				if (value.Type == VariantType.Integer)
				{
					// it's a flag
					nFlag = nFlags.GetInt(context);
					if (nFlag > 31 || nFlag < 0)
						throw new ErrorException(ErrorCodes.InvalidArgument);

					if (nFlag == 0)
						nFlag = 1;
					nFirstParser = 3;
				}
				else if (value.Type == VariantType.Character)
				{
					// the first param in that list isn't the flag but a parsing string
					nFirstParser = 2;
				}
				else // TODO check if third param can be not car and not int...
					throw new ErrorException(ErrorCodes.DataTypeInvalid,
							nFlags.GetName(context).ToUpper(System.Globalization.CultureInfo.InvariantCulture));

				// remove the unwished parameters
				param.RemoveRange(0, nFirstParser);
			}

			// get a string array of params[nFirstParse].GetString(context);
			string[] cResult = { "" };

			ArrayMember arr;
			if(array is WriteArray)
				arr = ((WriteArray)array).GetArray(context);
			else if (array is ArrayMemberAccess)
				arr = ((ArrayMemberAccess) array).GetArray(context);
			else
				arr = null;
			if (arr == null)
				throw new ErrorException(ErrorCodes.NotAnArray, array.GetName(context).ToUpper(System.Globalization.CultureInfo.InvariantCulture));

			// get the parsers
			var cParam = new string[param.Count];
			for (loop = 0; loop < param.Count; loop++)
				cParam[loop] = param[loop].GetString(context);

			// check flags...
			bool bRemoveLeadings = (nFlag & 1) != 0;
			bool bIncludeLastEmpty = (nFlag & 2) != 0;
			bool bRemoveEmpty = (nFlag & 4) != 0;
			bool bCanseInsensitive = (nFlag & 8) != 0;
			bool bIncludeParser = (nFlag & 16) != 0;

			// let's do it !
			Int32 index = 0;

			if (cParam.Length == 0)
			{
				cParam = new String[3];
				cParam[0] = "\r\n";
				cParam[1] = "\r";
				cParam[2] = "\n";
			}


			StringComparison sc;
			if (bCanseInsensitive)
				sc = StringComparison.CurrentCultureIgnoreCase;
			else
				sc = StringComparison.CurrentCulture;


			Int32 startPos = 0;
			while (startPos < cExpr.Length)
			{
				// searching all parsers in cExpr
				for ( Int32 i=0; i<cParam.Length; i++)
				{
					string str = cParam[i];
					Int32 pos = cExpr.IndexOf(str, startPos, sc);
					if(pos == -1 && i+1 == cParam.Length)
					{
						pos = cExpr.Length;
					}
					if (pos >= 0)
					{
						if ((pos == 0 && !bRemoveEmpty) || !bRemoveEmpty)
						{
							cResult = ChangeArraySize(cResult, index + 1);
							cResult.SetValue(cExpr.Substring(startPos, pos-startPos), index);

							if (bIncludeParser)
								cResult[index] += str;

							if (bRemoveLeadings)
								cResult[index].Trim();
							index++;
						}
						startPos = pos + str.Length;
						break;
					}
				}
			}


			// Copy the results info into array
			loop = cResult.GetLength(0);
			arr.Dimension(loop + 1);

			loop = 0;
			foreach (string result in cResult)
			{
				loop++;
				arr.Locate(1,loop).SetString(result);
			}
			return new Variant(loop, 11);
		}

	}
}
