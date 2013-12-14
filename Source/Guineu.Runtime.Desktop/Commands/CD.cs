using System;
using Guineu.Expression;

namespace Guineu
{

	class CD : ICommand
	{
		ExpressionBase _Directory;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			_Directory = Comp.GetCompiledExpression();
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			GuineuInstance.FileMgr.CurrentDirectory = _Directory.GetString(context);
		}
	}

}