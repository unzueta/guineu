using System;
using System.IO;
using Guineu.Core.FileSystem;

namespace Guineu.Core
{
	/// <summary>
	/// Encapsulates file access
	/// </summary>
	public partial class FileManager
	{
		Boolean supportsDirectories = true;
		readonly VirtualFileSystem vfs;

		String currentDirectoryField;
		public String CurrentDirectory
		{
			get
			{
				if (supportsDirectories)
					return Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
				return currentDirectoryField;
			}
			set
			{
				if (supportsDirectories)
					Directory.SetCurrentDirectory(value);
				else
					currentDirectoryField = value;
			}
		}

		public Boolean SupportCurrentDirectory
		{
			get { return supportsDirectories; }
		}

		// ReSharper disable PartialMethodWithSinglePart
		partial void DoSetDirectorySupport();
		// ReSharper restore PartialMethodWithSinglePart

		public FileManager()
		{
			vfs = new VirtualFileSystem();
			DoSetDirectorySupport();
		}

		/// <summary>
		/// Returns the full path to a file respecting the current SET PATH to setting.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="checkWindowsPath">
		/// When true searches the file not only in Guineu specifc paths, but also in every
		/// Windows path.
		/// </param>
		/// <returns></returns>
		public String FullPath(String file, Boolean checkWindowsPath)
		{
			//return file;
			String name;

			// Is the file available as a resource?
			// Does the file exist in the current directory?
			if (currentDirectoryField == null)
				name = vfs.GetFullPath(file);
			else
				name = Path.Combine(currentDirectoryField, file);
			// name = Path.Combine("c:\\", file);
			if (File.Exists(name))
				return name;

			// If a file is referenced from code we always look relative to that code file.
			// This is different from what VFP does most of the time, but sometimes we see
			// the same behavior in VFP, as well.
			if (GuineuInstance.CallingContext != null)
			{
				var currentCode = GuineuInstance.CallingContext.Stack.FileName;
				if (!String.IsNullOrEmpty(currentCode))
				{
					var dir = Path.GetDirectoryName(currentCode);
					name = Path.Combine(dir, file);
					if (File.Exists(name))
						return name;
				}
			}

			// Does it exist anywhere in the VFP path?
			foreach (String path in GuineuInstance.Set.Path)
			{
				name = Path.Combine(path, file);
				if (File.Exists(name))
				{
					return name;
				}
			}

			// Does it exist anywhere in the Windows path?
			if (checkWindowsPath)
			{
			}

			// default is the current directory
			if (currentDirectoryField == null)
				return vfs.GetFullPath(file);

			return Path.Combine(currentDirectoryField, file);
		}

		/// <summary>
		/// Returns a path suitable for the operating system and built-in file functions.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		/// <remarks>
		/// Windows Mobile does not support a current directory. All paths have to be
		/// specified as absolute paths.
		/// </remarks>
		public String MakePath(String file)
		{
			if (currentDirectoryField == null)
				return file;

			return Path.Combine(currentDirectoryField, file);
		}


		/// <summary>
		/// Returns true when a file exists.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public Boolean Exists(String file)
		{
			return LocateFile(file).Exists();
		}

		/// <summary>
		/// Opens a file. The file is located according to the active rules.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public Stream Open(String path, FileMode mode)
		{
			return LocateFile(path).Open(mode);
		}


		internal FileLocation LocateFile(String path)
		{
			String resourceName = GuineuInstance.FileNameToResourceName(path);
			if (!String.IsNullOrEmpty(resourceName))
				return new FileLocationEmbedded(GuineuInstance.CurrentExecutable, resourceName);

			return new FileLocationExternal(FullPath(path, false));
		}
	}

	internal abstract partial class FileLocation
	{
		abstract public ExecutableContext Executable
		{
			get;
		}
		abstract public Stream Open(FileMode mode);
		abstract public Boolean Exists();
		abstract public String PhysicalFile
		{
			get;
		}
	}

	internal partial class FileLocationEmbedded : FileLocation
	{
		readonly ExecutableContext storage;
		readonly String resourceName;

		public FileLocationEmbedded(ExecutableContext source, String name)
		{
			storage = source;
			resourceName = name;
		}

		public override Stream Open(FileMode mode)
		{
			return Open();
		}
		public override ExecutableContext Executable
		{
			get { return storage; }
		}
		public override bool Exists()
		{
			return true;
		}
		public override string PhysicalFile
		{
			get { return null; }
		}

		private Stream Open()
		{
			if (storage == null)
				return GuineuInstance.ServerExecutable.GetManifestResourceStream(resourceName);

			return storage.GetManifestResourceStream(resourceName);
		}
	}

	internal partial class FileLocationExternal : FileLocation
	{
		readonly String fileName;

		public FileLocationExternal(String path)
		{
			fileName = path.Replace('\\', Path.DirectorySeparatorChar);
		}

		public override ExecutableContext Executable
		{
			get { return null; }
		}
		public override Stream Open(FileMode mode)
		{
			String fullName;
			if (mode == FileMode.Create || mode == FileMode.CreateNew)
			{
				fullName = fileName;
			}
			else
			{
				fullName = GuineuInstance.FileMgr.FullPath(fileName, false);
			}
			return new FileStream(fullName, mode);
		}
		public override bool Exists()
		{
			return File.Exists(fileName);
		}
		public override string PhysicalFile
		{
			get { return fileName; }
		}

		private static String FullName(String path, FileMode mode)
		{
			if (mode == FileMode.Create || mode == FileMode.CreateNew)
				return path;
		
			return GuineuInstance.FileMgr.FullPath(path, false);
		}
	}
}
