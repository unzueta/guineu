using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Guineu.Expression;

namespace Guineu
{

	partial class ERASE : ICommand
	{
		ExpressionBase fileName;
		Boolean recycle;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.RECYCLE:
						code.Reader.ReadToken();
						recycle = true;
						break;

					default:
						fileName = Comp.GetCompiledExpression();
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			// TODO: NULL is not allowed
			String name = fileName.GetString(context);
			EraseFile(name, recycle);
		}

		static internal void EraseFile(String name, Boolean recycle)
		{
			if (name == "?" || String.IsNullOrEmpty(name))
				name = GuineuInstance.WinMgr.ShowOpenFileDialog();
			if (String.IsNullOrEmpty(name))
				return;
			name = GuineuInstance.FileMgr.MakePath(name);

			if (recycle)
			{
				var fileop = new SHFILEOPSTRUCT();
				fileop.wFunc = FO_DELETE;
				fileop.pFrom = name + '\0' + '\0';
				fileop.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;

				SHFileOperation(ref fileop);
			}
			else
				try
				{
					var dir = Path.GetDirectoryName(name);
					if (String.IsNullOrEmpty(dir))
						dir = GuineuInstance.FileMgr.CurrentDirectory;

					var files = Directory.GetFiles(dir, Path.GetFileName(name));
					foreach (var n in files)
					{
						File.Delete(n);
					}
				}
				catch (DirectoryNotFoundException)
				{
					// TODO: Display "File not found" message in statusbar
				}
				catch (IOException)
				{
					throw new ErrorException(ErrorCodes.FileIsInUse);
				}
		}
		private const int FO_DELETE = 3;
		private const int FOF_ALLOWUNDO = 0x40;
		private const int FOF_NOCONFIRMATION = 0x0010;

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);
	}
}