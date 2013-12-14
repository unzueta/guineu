using System;
using Guineu.Expression;

namespace Guineu.Commands
{
	class FunctionCall : ICommand
	{
		ExpressionBase function;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			function = comp.GetCompiledExpression();
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			function.GetVariant(context);
		}
	}
}
