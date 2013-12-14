using System;
using System.IO;

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

		/// <summary>
		/// Opens a file. The file is located according to the active rules.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="mode"></param>
		/// <param name="acc"></param>
		/// <param name="share"></param>
		/// <param name="buffer"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Stream Open(String path, FileMode mode, FileAccess acc, FileShare share, Int32 buffer, FileOptions options)
		{
			return LocateFile(path).Open(mode, acc, share, buffer, options);
		}
	}

	internal abstract partial class FileLocation
	{
		abstract public Stream Open(FileMode mode, FileAccess acc, FileShare share);
		abstract public Stream Open(FileMode mode, FileAccess acc, FileShare share, Int32 buffer, FileOptions options);
	}

	internal partial class FileLocationEmbedded
	{
		public override Stream Open(FileMode mode, FileAccess acc, FileShare share)
		{
			return Open();
		}
		public override Stream Open(FileMode mode, FileAccess acc, FileShare share, int buffer, FileOptions options)
		{
			return Open();
		}
	}

	internal partial class FileLocationExternal
	{
		public override Stream Open(FileMode mode, FileAccess acc, FileShare share)
		{
			return Open(mode, acc, share, 4096, FileOptions.None);
		}
		public override Stream Open(FileMode mode, FileAccess acc, FileShare share, int buffer, FileOptions options)
		{
			try
			{
				return new FileStream(FullName(fileName, mode), mode, acc, share, buffer, options);
			}
			catch (FileNotFoundException)
			{
				throw new ErrorException(ErrorCodes.FileNotFound);
			}
			catch(UnauthorizedAccessException)
			{
				throw new ErrorException(ErrorCodes.FileAccessDenied);
			}
		}
	}

}
