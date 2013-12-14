using System;
using Guineu.Expression;
using System.Collections.Generic;

namespace Guineu.Functions
{
    /// <summary>
    /// HTTP access
    /// </summary>
    partial class SYS8012 : ISys
    {
        static ISipController Controller;

        public String getString(CallingContext context, List<ExpressionBase> param)
        {
            KnownNti cmd = param[1].ToNti(context).ToKnownNti();
            String retVal = "";
            DoLoadSipController();

            switch (cmd)
            {
                case KnownNti.On:
                    Controller.Enabled = true;
                    break;

                case KnownNti.Off:
                    Controller.Enabled = false;
                    break;

                case KnownNti.Status:
                    retVal = Controller.Enabled ? "ON" : "OFF";
                    break;

                case KnownNti.Current:
                    retVal = Controller.Current;
                    if (param.Count >= 3)
                        Controller.Current = param[2].GetString(context);
                    break;

                case KnownNti.Available:
                    retVal = Controller.Available;
                    break;

                default:
                    break;
            }

            return retVal;
        }

        static partial void DoLoadSipController();
    }

    internal interface ISipController
    {
        Boolean Enabled { get; set; }
        String Current { get; set; }
        String Available { get; }
    }
}