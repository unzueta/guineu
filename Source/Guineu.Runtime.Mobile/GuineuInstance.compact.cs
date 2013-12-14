using System.Text;
using Guineu.Data;
using Guineu.Gui.Compact;

namespace Guineu
{
	public static partial class GuineuInstance
	{

		private static void PlatformSpecificInit()
		{
			Connections.Engine = new CompactEngine();
			WinMgr = new CompactManager();
			CurrentCp = Encoding.GetEncoding(1252);
		}
	}
}
