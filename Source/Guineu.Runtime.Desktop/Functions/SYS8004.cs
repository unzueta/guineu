using System;
using System.Collections.Generic;
using Guineu.Expression;
using Guineu.Data;

namespace Guineu.Functions
{
    partial class SYS8004 : ISys
	{
		/// <summary>
		/// Switches the SPT engine
		/// </summary>
		/// <returns></returns>
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			// Query the current state
		    string retVal = GuineuInstance.Connections.Engine.Name;

			// Change the current engine
			if (param.Count >= 2)
			{
				String name = param[1].GetString(context);
				if (!PlatformSpecificEngines(name))
					SetEngine(name);
			}

			return retVal;
		}

		static void SetEngine(String name)
		{
			switch (name)
			{
				case "compact":
					GuineuInstance.Connections.Engine = new CompactEngine();
					break;
			}
		}

	}
}