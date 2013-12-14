using System.Collections.Generic;
using Guineu.Core;
using Guineu.Expression;

namespace Guineu.Functions
{
    class FCHSIZE : ExpressionBase
    {
        ExpressionBase handleExpression;
        ExpressionBase newSizeExpression;

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
                    newSizeExpression = param[1];
                    break;
                default:
                    throw new ErrorException(ErrorCodes.TooManyArguments);
            }
            FixedInt = true;
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
            if (newSizeExpression != null)
            {
                value = newSizeExpression.GetVariant(context);
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

            return new Variant(GetInt(context), 10);
        }

        internal override int GetInt(CallingContext context)
        {
            int guineuHandle = handleExpression.GetInt(context);
            int nNewSize = newSizeExpression.GetInt(context);

            LowLevelFileMngr llfmngr = GuineuInstance.FFilesManager;
            var retVal = (int)llfmngr.FChSize(guineuHandle, nNewSize);

            return retVal;
        }
    }
}
