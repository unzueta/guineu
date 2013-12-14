using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
    class STREXTRACT : ExpressionBase
    {
        ExpressionBase searchExpression;
        ExpressionBase beginDelimExpression;
        ExpressionBase endDelimExpression;
        ExpressionBase occurrenceExpression;
        ExpressionBase flagExpression;

        private int paramCount;

        override internal void Compile(Compiler comp)
        {
            List<ExpressionBase> param = comp.GetParameterList();
            paramCount = param.Count;

            switch (paramCount)
            {
                case 0:
                    throw new ErrorException(ErrorCodes.TooFewArguments);
                case 1:
                    throw new ErrorException(ErrorCodes.TooFewArguments);
                case 2:
                    searchExpression = param[0];
                    beginDelimExpression = param[1];
                    break;
                case 3:
                    searchExpression = param[0];
                    beginDelimExpression = param[1];
                    endDelimExpression = param[2];
                    break;
                case 4:
                    searchExpression = param[0];
                    beginDelimExpression = param[1];
                    endDelimExpression = param[2];
                    occurrenceExpression = param[3];
                    break;

                case 5:
                    searchExpression = param[0];
                    beginDelimExpression = param[1];
                    endDelimExpression = param[2];
                    occurrenceExpression = param[3];
                    flagExpression = param[4];
                    break;

                default:
                    throw new ErrorException(ErrorCodes.TooManyArguments);
            }
        }

        override internal Variant GetVariant(CallingContext context)
        {
            Variant value;

						if (searchExpression.CheckString(context, false))
							return new Variant(VariantType.Character, true);

						if (beginDelimExpression.CheckString(context, false))
							return new Variant(VariantType.Character, true);

						if (endDelimExpression.CheckString(context, false))
							return new Variant(VariantType.Character, true);
            
            // Check parameter #4
            if (occurrenceExpression != null)
            {
                value = occurrenceExpression.GetVariant(context);
                if (value.IsNull)
                {
                    throw new ErrorException(ErrorCodes.InvalidArgument);
                }
                // Only int is a valid parameter
                if (value.Type != VariantType.Integer)
                {
                    throw new ErrorException(ErrorCodes.InvalidArgument);
                }
            }

            // Check parameter #5
            if (flagExpression != null)
            {
                value = flagExpression.GetVariant(context);
                if (value.IsNull)
                    throw new ErrorException(ErrorCodes.InvalidArgument);
                // Only int is a valid parameter
                if (value.Type != VariantType.Integer)
                    throw new ErrorException(ErrorCodes.InvalidArgument);
                // the flag can't be greater than 7 (range : 0 to 7)
                Int32 iVal = value;
                if (iVal.CompareTo(8) >= 0 || iVal.CompareTo(0) < 0)
                    throw new ErrorException(ErrorCodes.InvalidArgument);
            }
            return new Variant(GetString(context));
        }


        internal override string GetString(CallingContext context)
        {
					if (searchExpression.CheckString(context, false))
						return null;

					if (beginDelimExpression.CheckString(context, false))
						return null;

					if (endDelimExpression.CheckString(context, false))
						return null;

					return GetExtractString(context);
        }

        private string GetExtractString(CallingContext context)
        {
            switch (paramCount)
            {
                case 2:
                    // call the sub routine with default 3,4,5 params...
                    return StrExtract(searchExpression.GetString(context), beginDelimExpression.GetString(context), "", 1, 0);
                case 3:
                // call the sub routine with default  4,5 params...
                    return StrExtract(searchExpression.GetString(context), beginDelimExpression.GetString(context), endDelimExpression.GetString(context), 1, 0);
                case 4:
                // call the sub routine with default 5 param...
                    return StrExtract(searchExpression.GetString(context), beginDelimExpression.GetString(context), endDelimExpression.GetString(context), occurrenceExpression.GetInt(context), 0);
                case 5:
                // call the sub routine ...
                    return StrExtract(searchExpression.GetString(context), beginDelimExpression.GetString(context), endDelimExpression.GetString(context), occurrenceExpression.GetInt(context), flagExpression.GetInt(context));
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
            for(Int32 loop = 0; loop < count; loop++)
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

        private static string StrExtract(string cSearchExpression, string cBeginDelim, string cEndDelim, int nBeginOccurence, int nFlags)
        {
            string cstring = cSearchExpression;
            string cb = cBeginDelim;
            string ce = cEndDelim;

            string lcRetVal = "";

            if (cstring.Length > 0)
            {
                //Check for case-sensitive or insensitive search
                if ((nFlags & 1) != 0)
                {
                    cstring = cstring.ToLower();
                    cb = cb.ToLower();
                    ce = ce.ToLower();
                }

                //Lookup the position in the string
                int nbpos = At(cb, cstring, nBeginOccurence) + cb.Length - 1;
            	int nepos;
							if (nbpos + 1 > cstring.Length)
								nepos = 0;
							else
                nepos = cstring.IndexOf(ce, nbpos + 1, StringComparison.InvariantCulture);

                // check if we can ignore the last sep
                if ((nepos <= nbpos && (nFlags & 2) != 0) || ce.Equals(""))
                {
                    nepos = cSearchExpression.Length;
                }

                //Extract the part of the string if we get it right
                if (nepos > nbpos)
                {
                    lcRetVal = cSearchExpression.Substring(nbpos, nepos - nbpos);
                }
            }

            // potentially, add the begin and end delimiter
            if ((nFlags & 4)!=0)
            {
                lcRetVal = cBeginDelim + lcRetVal;
                lcRetVal += cEndDelim;
            }

            return lcRetVal;
        }

    }
}
