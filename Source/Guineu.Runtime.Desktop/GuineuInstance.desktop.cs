using System.Text;
using Guineu.Data.Engines.Spt.Odbc;
using Guineu.Data;

namespace Guineu
{
	public static partial class GuineuInstance
	{
		static void PlatformSpecificInit()
		{
			Connections = new SptConnectionManager {Engine = new OdbcEngine()};
			if(WinMgr == null)
				WinMgr = new Gui.Desktop.DesktopManager();
				CurrentCp = Encoding.GetEncoding(1252);
		}
	}
}