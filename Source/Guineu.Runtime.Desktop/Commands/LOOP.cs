using System;

namespace Guineu.Commands
{
	class LOOP : ICommand
	{
		Int32 JumpTo;

		public void Compile(CodeBlock code)
		{
			var fce = code.ControlFlowStack.PeekLastLoop();
			if (fce == null) 
				throw new ErrorException(ErrorCodes.NestingError);
			JumpTo = fce.Line;
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			nextLine = JumpTo;
		}
	}
}
