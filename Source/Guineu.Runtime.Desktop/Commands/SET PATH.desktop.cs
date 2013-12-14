using System;

namespace Guineu.Commands
{
	public partial class SetPathValue
	{
		partial void ParsePath()
		{
			pathList.Clear();
			String[] pl = path.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < pl.Length; i++)
			{
				pathList.Add( pathList[i].Trim());
			}
		}

	}

}
