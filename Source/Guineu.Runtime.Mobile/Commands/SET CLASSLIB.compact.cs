using System;
using System.Globalization;

namespace Guineu.Commands
{
	partial class ManagedClassLibrary
	{
		Type GetTypeFromFile(String name)
		{
			var uName = name.ToUpper(CultureInfo.InvariantCulture);
			Type type = null;
			foreach (var t in file.GetTypes())
			{
				if (t.FullName.ToUpper(CultureInfo.InvariantCulture) == uName)
				{
					type = t;
					break;
				}
			}
			return type;
		}
	}
}