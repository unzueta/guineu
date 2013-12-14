using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu.Functions
{
    class FDATE : ExpressionBase
    {
        ExpressionBase fileNameExpression;

        override internal void Compile(Compiler comp)
        {
            List<ExpressionBase> param = comp.GetParameterList();
            switch (param.Count)
            {
                case 0:
                    throw new ErrorException(ErrorCodes.TooFewArguments);
                case 1:
                    fileNameExpression = param[0];
                    break;
                default:
                    throw new ErrorException(ErrorCodes.TooManyArguments);
            }
        }

        override internal Variant GetVariant(CallingContext context)
        {
            if (fileNameExpression.CheckString(context, false))
                throw new ErrorException(ErrorCodes.InvalidArgument);

						string fileName = fileNameExpression.GetString(context);
						String name = GuineuInstance.FileMgr.MakePath(fileName);
						DateTime dRetVal = DateTime.Today;
						try
						{
							if (File.Exists(name))
							{
								var fi = new FileInfo(name);
								dRetVal = fi.LastWriteTime;
							}
						}
						catch (Exception)
						{
							throw new ErrorException(ErrorCodes.FileNotFound);
						}

						return new Variant(dRetVal, VariantType.Date);
        }
    }
}
