using System;

namespace Guineu.Commands
{
	partial class ManagedClassLibrary
	{
		Type GetTypeFromFile(String name)
		{
			var type = file.GetType(name, false, true);
			return type;
		}
	}
}