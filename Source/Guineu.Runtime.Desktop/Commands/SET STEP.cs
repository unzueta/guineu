using System;
using Guineu.Expression;

namespace Guineu.Commands
{
	class SETSTEP : ICommand
	{
		Boolean showDebugger;

		public void Compile(CodeBlock code)
		{
			Token nextToken;
			do
			{
				nextToken = code.Reader.ReadToken();
				switch (nextToken)
				{
					case Token.ON:
						showDebugger = true;
						break;
					case Token.OFF:
						showDebugger = false;
						break;
					case Token.CmdEnd:
						break;
				}
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			if (showDebugger)
				if (System.Diagnostics.Debugger.IsAttached)
					System.Diagnostics.Debugger.Break();
		}
	}
}
