using System.Text;

namespace Guineu
{
	public static partial class GuineuInstance
	{
		private static void PlatformSpecificInit()
		{
            CurrentCp = Encoding.GetEncoding("utf-8");
        }
	}
}
