using System;
using Guineu.Expression;

namespace Guineu
{

	class DOWHILE : ICommand
	{
		ExpressionBase condition;
		int jumpIfFalse;
	
		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			condition = comp.GetCompiledExpression();
			jumpIfFalse = code.GetLineAtPosition(comp.ReadInt());
			code.ControlFlowStack.Push(code.CurrentLine, FlowControlEntry.Types.DoWhile,jumpIfFalse+1);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			if (!condition.GetBool(context))
			{
				nextLine = jumpIfFalse + 1;
			}
		}
	}

}
