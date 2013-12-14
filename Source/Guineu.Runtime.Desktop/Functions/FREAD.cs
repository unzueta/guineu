using System.Collections.Generic;
using Guineu.Core;
using Guineu.Expression;

namespace Guineu.Functions
{
    class FREAD : ExpressionBase
    {
        ExpressionBase handleExpression;
        ExpressionBase bytesExpression;

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
                    bytesExpression = param[1];
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

            // Check parameter #2
            if (bytesExpression != null)
            {
                value = bytesExpression.GetVariant(context);
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

            return new Variant(GetString(context));
        }

        internal override string GetString(CallingContext context)
        {
            int guineuHandle = handleExpression.GetInt(context);
            int nBytes = bytesExpression.GetInt(context);

            LowLevelFileMngr llfmngr = GuineuInstance.FFilesManager;
            string retVal = llfmngr.Fread(guineuHandle, nBytes);

            return retVal;
        }
    }
}
