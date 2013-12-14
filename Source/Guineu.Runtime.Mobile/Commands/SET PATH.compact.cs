using System;

namespace Guineu.Commands
{
	public partial class SetPathValue
	{
		partial void ParsePath()
		{
			pathList.Clear();
			String[] pl = path.Split(new Char[] { ',', ';' });
			for (int i = 0; i < pl.Length; i++)
			{
				pathList.Add(pathList[i].Trim());
			}
		}

	}
}
