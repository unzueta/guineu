using System;
using Guineu.Data;
using Guineu.Data.Engines.Spt.Odbc;

namespace Guineu.Functions
{
	 partial class SYS8004
	{
        Boolean PlatformSpecificEngines(String name)
		{
			switch (name)
			{
				case "odbc":
					GuineuInstance.Connections.Engine = new OdbcEngine();
					return true;
				case "context":
					GuineuInstance.Connections.Engine = new ContextConnectionEngine();
					return true;
			}
			return false;
		}

	 	
	}
}