using System;
using System.Collections.Generic;
using Guineu.Expression;
using Guineu.Gui.Compact;

namespace Guineu.Functions

{
    class SYS8006 : ISys
	{
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			var mgr = GuineuInstance.WinMgr as CompactManager;
		    if (mgr != null)
		    {
		        var retVal = mgr.Scale.ToString();
		        if (param.Count > 0)
		        {
		            var scale = param[1].GetDouble(context);
		            mgr.Scale = scale;
		        }
		        return retVal;
		    }
		    return "";
		}
	}

}