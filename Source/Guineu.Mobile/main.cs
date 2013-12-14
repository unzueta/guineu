using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Guineu.compact
{
	static class main
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[MTAThread]
		static void Main(string[] args)
		{
			// Initialize Guineu
			GuineuInstance.InitInstance();

			// Determine which FXP file Guineu should execute. This is either the first parameter
			// or a file that has the same name as the EXE.
			String filename = GetFileName(args);

			// Since some PDAs don't have any concept of a current directory, we start
			// relativ to the FXP in this case.
			if (filename != null)
				if (!GuineuInstance.FileMgr.SupportCurrentDirectory)
				{
					GuineuInstance.FileMgr.CurrentDirectory = Path.GetDirectoryName(filename);
				}
	
			// execute the FXP file
			if (filename != null)
			{
				GuineuInstance.Do(filename);
			}
		}

		static String GetFileName(string[] args)
		{
			String filename;
			if (args.Length == 0)
			{
				filename = Path.ChangeExtension(Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName, "FXP");
				if (!GuineuInstance.FileMgr.Exists(filename))
				{
					var dlg = new OpenFileDialog
					          	{
					          		Filter = "Executables (*.fxp)|*.fxp|All files (*.*)|*.*"
					          	};
					filename = dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
				}
			}
			else
			{
				filename = args[0];
			}
			return filename;
		}
	}
}