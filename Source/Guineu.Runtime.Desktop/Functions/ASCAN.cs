using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
    /// <summary>
    /// ASCAN()
    /// </summary>
    partial class ASCAN : ExpressionBase
    {
        ExpressionBase _ArrayName;
        ExpressionBase _eExpression;
        
        ExpressionBase _nStartElement;
        ExpressionBase _nElementsSearched;
        ExpressionBase _nSearchColumn;
        ExpressionBase _nFlags;
/*
        0 Case Insensitive bit 
        1 Exactness ON bit (Only effective if bit 2 is set)
        2 Override system Exact setting bit
        3 Return row number if 2D array
*/
        override internal void Compile(Compiler comp)
        {
            List<ExpressionBase> param = comp.GetParameterList();
            switch (param.Count)
            {
                case 0:
                case 1:
                    throw new ErrorException(ErrorCodes.TooFewArguments);
                case 2:
                    _ArrayName = param[0];
                    _eExpression = param[1];
                    break;
                case 3:
                    _ArrayName = param[0];
                    _eExpression = param[1];
                    _nStartElement = param[2];
                    break;
                case 4:
                    _ArrayName = param[0];
                    _eExpression = param[1];
                    _nStartElement = param[2];
                    _nElementsSearched = param[3];
                    break;
                case 5:
                    _ArrayName = param[0];
                    _eExpression = param[1];
                    _nStartElement = param[2];
                    _nElementsSearched = param[3];
                    _nSearchColumn = param[4];
                    break;
                case 6:
                    _ArrayName = param[0];
                    _eExpression = param[1];
                    _nStartElement = param[2];
                    _nElementsSearched = param[3];
                    _nSearchColumn = param[4];
                    _nFlags = param[5];
                    break;
                default:
                    throw new ErrorException(ErrorCodes.TooManyArguments);
            }
            FixedInt = true;
        }

        override internal Variant GetVariant(CallingContext context)
        {
            Int64 pos = 0;
/*            Int64 from = 0;
            Int64 to = 0;
            Int64 colNum = 0;
            
            int nFlags = 0;

            // Get the array
            String arrayName = _ArrayName.GetName(context);
            ArrayMember arr = context.Resolver.Resolve(context, arrayName) as ArrayMember;
            if (arr == null)
            {
                throw new ErrorException(ErrorCodes.NotAnArray, arrayName.ToUpper(System.Globalization.CultureInfo.InvariantCulture));
            }

            eExpression = _eExpression.GetVariant(context);
            
            // ok, get the params, then the subscript array we have to search in
            if (colNum == 0)
            { // when we have all parameters except column
                pos = arr.FindIndex(arr, from, to, CheckVariant);
                pos++;
            }
            else
            {
                // ????????????????????????
            }
*/
            return new Variant((int)pos, 10);
        }
/*
        private bool CheckVariant(ValueMember e)
        {
            return false;
            // return eExpression.IsEqual(get_that_bloody_variant_from_valuemember(e));
        }
*/
    }

}