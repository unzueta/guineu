using System.Collections.Generic;
using Guineu.Core;
using Guineu.Expression;

namespace Guineu.Functions
{
    class FPUTS : ExpressionBase
    {
        ExpressionBase handleExpression;
        ExpressionBase textExpression;
        ExpressionBase lengthExpression;

        override internal void Compile(Compiler comp)
        {
            List<ExpressionBase> param = comp.GetParameterList();
            switch (param.Count)
            {
                case 0:
                    throw new ErrorException(ErrorCodes.TooFewArguments);
                case 1:
                    handleExpression = param[0];
                    break;
                case 2:
                    handleExpression = param[0];
                    textExpression = param[1];
                    break;
                case 3:
                    handleExpression = param[0];
                    textExpression = param[1];
                    lengthExpression = param[2];
                    break;
                default:
                    throw new ErrorException(ErrorCodes.TooManyArguments);
            }
        }

        override internal Variant GetVariant(CallingContext context)
        {
            // Check parameter #1, fileHandle
            Variant value = handleExpression.GetVariant(context);
            if (value.IsNull)
            {
                throw new ErrorException(ErrorCodes.InvalidArgument);
            }
            // Only strings are valid parameters
            if (value.Type != VariantType.Integer)
            {
                throw new ErrorException(ErrorCodes.InvalidArgument);
            }

            // param 2, cExpression
            if (textExpression.CheckString(context, false))
                throw new ErrorException(ErrorCodes.InvalidArgument);

            // param3, bytes to put
            if (lengthExpression != null)
            {
                value = lengthExpression.GetVariant(context);
                if (value.IsNull)
                {
                    throw new ErrorException(ErrorCodes.InvalidArgument);
                }
                // Only strings are valid parameters
                if (value.Type != VariantType.Integer)
                {
                    throw new ErrorException(ErrorCodes.InvalidArgument);
                }
            }

            return new Variant(GetInt(context), 10);
        }

        internal override int GetInt(CallingContext context)
        {
            int guineuHandle = handleExpression.GetInt(context);
            string cExpression = textExpression.GetString(context);
            int nCar = lengthExpression == null ? 0 : lengthExpression.GetInt(context);

            LowLevelFileMngr llfmngr = GuineuInstance.FFilesManager;
            var retVal = (int)llfmngr.Fputs(guineuHandle, cExpression, nCar);

            return retVal;
        }
    }
}
