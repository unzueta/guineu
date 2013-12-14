using System;
using Guineu.Expression;

namespace Guineu.Functions
{
    class FCREATE : ExpressionBase
    {
        ExpressionBase fileNameExpression;
        ExpressionBase flagExpression;

        override internal void Compile(Compiler comp)
        {
            var param = comp.GetParameterList();
            switch (param.Count)
            {
                case 0:
                    throw new ErrorException(ErrorCodes.TooFewArguments);
                case 1:
                    fileNameExpression = param[0];
                    break;
                case 2:
                    fileNameExpression = param[0];
                    flagExpression = param[1];
                    break;
                default:
                    throw new ErrorException(ErrorCodes.TooManyArguments);
            }
            FixedInt = true;
        }

        override internal Variant GetVariant(CallingContext context)
        {
            if (fileNameExpression.CheckString(context, false))
                throw new ErrorException(ErrorCodes.InvalidArgument);

            // Check parameter #2
            Variant value;
            if (flagExpression != null)
            {
                value = flagExpression.GetVariant(context);
                if (value.IsNull)
                    throw new ErrorException(ErrorCodes.InvalidArgument);
                // Only int is a valid parameter
                if (value.Type != VariantType.Integer)
                    throw new ErrorException(ErrorCodes.InvalidArgument);
                // the flag can't be greater than 2 (range : 0 to 2)
                Int32 iVal = value;
                if (iVal.CompareTo(3) >= 0 || iVal.CompareTo(0) < 0)
                    throw new ErrorException(ErrorCodes.InvalidArgument);
            }

            return new Variant(GetInt(context), 10);
        }

        internal override int GetInt(CallingContext context)
        {
            var fileName = fileNameExpression.GetString(context);
            fileName = GuineuInstance.FileMgr.MakePath(fileName);
            var nFlags = 0; // default
            if (flagExpression != null)
                nFlags = flagExpression.GetInt(context);

            var llfmngr = GuineuInstance.FFilesManager;
            var retVal = llfmngr.FCreate(fileName, nFlags);

            return retVal;
        }
    }
}
