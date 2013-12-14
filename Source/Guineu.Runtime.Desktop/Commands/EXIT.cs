using System;

namespace Guineu.Commands
{
	class EXIT : ICommand
	{
		Int32 jumpTo;

		public void Compile(CodeBlock code)
		{
			var fce = code.ControlFlowStack.PeekLastLoop();
			if (fce == null) 
				throw new ErrorException(ErrorCodes.NestingError);
			jumpTo = fce.AlternativeLine;
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			nextLine = jumpTo;
		}
	}

}
