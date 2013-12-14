using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
    class SYS0 : ISys
    {
        public string getString(CallingContext context, List<ExpressionBase> param)
        {
            //string deviceName = null;
            //int size = 128;

            //RegKey reg = new RegKey();
            //reg.Open(RegKey._HKEY_LOCAL_MACHINE, "Ident");
            //reg.QueryValue(out deviceName, "Name", ref size);
            //reg.Close();

            return string.Empty;
        }
    }
}

