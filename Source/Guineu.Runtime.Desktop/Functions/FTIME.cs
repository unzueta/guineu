using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

// TODO : VFP Date and DateTime type

namespace Guineu.Functions
{
    class FTIME : ExpressionBase
    {
        ExpressionBase fileNamExpression;

        override internal void Compile(Compiler comp)
        {
            List<ExpressionBase> param = comp.GetParameterList();
            switch (param.Count)
            {
                case 0:
                    throw new ErrorException(ErrorCodes.TooFewArguments);
                case 1:
                    fileNamExpression = param[0];
                    break;
                default:
                    throw new ErrorException(ErrorCodes.TooManyArguments);
            }
			}

        override internal Variant GetVariant(CallingContext context)
        {
            if (fileNamExpression.CheckString(context, false))
                throw new ErrorException(ErrorCodes.InvalidArgument);

            return new Variant(GetString(context));
        }

        internal override string GetString(CallingContext context)
        {
            string fileName = fileNamExpression.GetString(context);
            string cRetVal = string.Empty;

            try
            {
                //Check if it exists
                if (File.Exists(fileName))
                {
                    //Create the FileInfo object
                    var fi = new FileInfo(fileName);

                    //Call the LastAccessTime to get the last read/write/copy time
                    cRetVal = fi.LastWriteTime.ToShortTimeString();
                }
                //                else
                //                    SetError(fErrorCode.FilenotFound);

                return cRetVal;
            }
            catch (Exception)
            {
                //                SetError(fErrorCode.AccessDenied);
                return cRetVal;
            }

        }
    }
}
