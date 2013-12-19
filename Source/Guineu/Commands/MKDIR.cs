using System;
using System.IO;
using Guineu.Expression;

namespace Guineu.Commands
{

	class MKDIR : ICommand
	{
		ExpressionBase directory;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			directory = comp.GetCompiledExpression();
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			String name=directory.GetString(context);
			name = GuineuInstance.FileMgr.MakePath(name);
			Directory.CreateDirectory(name);
		}
	}
}