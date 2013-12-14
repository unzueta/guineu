using System;
using System.Reflection;
using System.IO;

namespace Guineu.Core
{
	public class ExecutableContext
	{
		readonly Assembly storage;
		String[] embeddedFiles;

		internal ExecutableContext(Assembly source)
		{
			storage = source;
			LoadEmbeddedFileArray();
		}

		private void LoadEmbeddedFileArray()
		{
			embeddedFiles = storage.GetManifestResourceNames();
		}

		internal Stream GetManifestResourceStream(string name)
		{
			return storage.GetManifestResourceStream(name);
		}
		internal string FileNameToResourceName(string fullPath)
		{
			String path = Path.GetFileName(fullPath);
			foreach (String name in embeddedFiles)
			{
                if (name.EndsWith("." + path, StringComparison.InvariantCultureIgnoreCase))
                    return name;
                if (name.EndsWith("." + path+".fxp", StringComparison.InvariantCultureIgnoreCase))
                    return name;
                if (String.Equals(name, path, StringComparison.InvariantCultureIgnoreCase))
					return name;
			}
			return null;
		}
	}
}
