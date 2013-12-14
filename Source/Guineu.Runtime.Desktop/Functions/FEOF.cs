using System.Collections.Generic;
using Guineu.Core;
using Guineu.Expression;

namespace Guineu.Functions
{
    class FEOF : ExpressionBase
    {
        ExpressionBase handleExpression;

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

            return new Variant(GetBool(context));
        }

        internal override bool GetBool(CallingContext context)
        {
            int guineuHandle = handleExpression.GetInt(context);

            LowLevelFileMngr llfmngr = GuineuInstance.FFilesManager;
            bool retVal = llfmngr.FEof(guineuHandle);

            return retVal;
        }

    }
}
