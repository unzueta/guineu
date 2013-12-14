using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
    class INKEY : ExpressionBase
    {
        ExpressionBase Time;

        override internal void Compile(Compiler comp)
        {
            List<ExpressionBase> param = comp.GetParameterList();
            switch (param.Count)
            {
                case 0:
                    break;
                case 1:
                    Time = param[0];
                    break;
                default:
                    throw new ErrorException(ErrorCodes.TooManyArguments);
            }
        }

        override internal Variant GetVariant(CallingContext context)
        {
            Double timeOut;
            if (Time == null)
                timeOut = 0;
            else
                timeOut = Time.GetDouble(context);

            // TODO: Make a loop waiting for the timeout
            var keyInfo = Console.ReadKey();
            Byte ch = GuineuInstance.CurrentCp.GetBytes(new char[] {keyInfo.KeyChar})[0];
            return new Variant((Int32) ch,10);
        }

    }

}