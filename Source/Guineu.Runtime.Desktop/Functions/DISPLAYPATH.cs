using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu.Functions
{
	class DISPLAYPATH : ExpressionBase
	{
		ExpressionBase path;
		ExpressionBase lengh;

		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
				case 1:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 2:
					path = param[0];
					lengh = param[1];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
		}

		override internal Variant GetVariant(CallingContext context)
		{
			var value = path.GetVariant(context);
		    if (value.Type != VariantType.Character)
		        throw new ErrorException(ErrorCodes.InvalidArgument);
		    value = lengh.GetVariant(context);
		    if (value.Type != VariantType.Number && value.Type != VariantType.Integer)
		        throw new ErrorException(ErrorCodes.InvalidArgument);
		    if (value < 10)
		        throw new ErrorException(ErrorCodes.InvalidArgument);
		    return new Variant(GetString(context));
		}

		internal override string GetString(CallingContext context)
		{	// based on GetShortenedPath found on the web...
			string cFullName = path.GetString(context);
			int nLen = lengh.GetInt(context);
			const string ellipsis = "...";

			if (String.IsNullOrEmpty(cFullName))
			{
				// is empty return empty
				return string.Empty;
			}
		    if (String.IsNullOrEmpty(cFullName.Trim()))
		    {
		        // if space, retrun curdir
		        cFullName = GuineuInstance.FileMgr.CurrentDirectory;
		    }

		    try
			{
				// if len ok return the path
				if (cFullName.Length <= nLen)
				{
					return cFullName;
				}

				// get the path parts
				char[] cSeparator = { Path.DirectorySeparatorChar };
			    string[] aStr = cFullName.Split(cSeparator);

				string lcBegin = aStr[0] + Path.DirectorySeparatorChar + ellipsis;
				int lnBeginLength = aStr[0].Length + ellipsis.Length;
				string lcRetVal = string.Empty;
				int lnLength = lcRetVal.Length;
				bool lAddHeader = false;

				string s;

			    //Now we loop backwards through the string
				int i;
				for (i = aStr.Length - 1; i > 0; i--)
				{
					s = Path.DirectorySeparatorChar + aStr[i];
					int n = s.Length;

					//Check if adding the current item does not increase the length of the max string
					if (lnLength + n <= nLen)
					{
						//In this case we can afford to add the item
						lcRetVal = s + lcRetVal;
						lnLength += n;
					}
					else
					{
						break;
					}

					//Check if there is room to add the header and if so then reserve it by incrementing the length
					if ((lAddHeader == false) && (lnLength + lnBeginLength <= nLen))
					{
						lAddHeader = true;
						lnLength += lnBeginLength;
					}
				}

				//Add the header if the bool is true
				if (lAddHeader)
				{
					lcRetVal = lcBegin + lcRetVal;
				}

				//It is possible that the last value in the array itself was long. In such case simply use the substring of the last value
				if (lcRetVal.Length == 0)
				{
					lcRetVal = aStr[aStr.Length - 1].Substring(0, nLen);
				}

				return lcRetVal;
			}
			catch (ArgumentException)
			{
				return cFullName;
			}
		}

	}
}
