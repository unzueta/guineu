using System;
using Guineu.Data.Engines.Spt.mssql;

namespace Guineu.Functions
{
	 partial class SYS8004
	 {
		static Boolean PlatformSpecificEngines(String name)
		{
			switch (name)
			{
				case "mssql":
					GuineuInstance.Connections.Engine = new MssqlEngine();
					return true;
			} return false;
		}
	}
}