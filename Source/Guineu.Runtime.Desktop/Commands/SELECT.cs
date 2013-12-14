using System;
using Guineu.Expression;

namespace Guineu.Commands
{
	class SELECT : ICommand
	{
		ExpressionBase alias;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			alias = comp.GetCompiledExpression();
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			Int32 area = exec.DataSession.WorkareaFromName(alias, exec);
			if (area == 0)
				area = exec.DataSession.GetNextFreeWorkarea();
			exec.DataSession.Select(area);
		}
	}
}
