using System;
using System.IO;
using System.Reflection;

namespace Guineu.Core
{
	/// <summary>
	/// Encapsulates file access
	/// </summary>
	public partial class FileManager
	{

		public Stream Open(String path, FileMode mode, FileAccess acc, FileShare share)
		{
			return LocateFile(path).Open(mode, acc, share);
		}

		partial void DoSetDirectorySupport()
		{
			supportsDirectories = false;
		} 
	}
	internal abstract partial class FileLocation
	{
		abstract public Stream Open(FileMode mode, FileAccess acc, FileShare share);
	}

	internal partial class FileLocationEmbedded : FileLocation
	{
		public override Stream Open(FileMode mode, FileAccess acc, FileShare share)
		{
			return Open();
		}
	}

	internal partial class FileLocationExternal : FileLocation
	{
		public override Stream Open(FileMode mode, FileAccess acc, FileShare share)
		{
			String fullName;
			if (mode == FileMode.Create || mode == FileMode.CreateNew)
				fullName = fileName;
			else
				fullName = GuineuInstance.FileMgr.FullPath(fileName, false);

			Stream s;
			try
			{
				s = new FileStream(fullName, mode, acc, share);
			}
			catch (UnauthorizedAccessException)
			{
				throw new ErrorException(ErrorCodes.FileAccessDenied, fullName);
			}
			return s;
		}
	}
}
