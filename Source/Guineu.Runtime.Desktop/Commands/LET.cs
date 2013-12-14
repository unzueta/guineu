using System;
using Guineu.Commands;

namespace Guineu
{
	class LET : ICommand
	{
		VariableAssignment var;

		public void Compile(CodeBlock code)
		{
			var = new VariableAssignment();
			var.Compile(code);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			var.Do(context);
		}
	}
}
