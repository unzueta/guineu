using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Guineu.Expression;

namespace Guineu
{
	class WAIT : ICommand
	{
		ExpressionBase message;
		Boolean window;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);

			Token nextToken = code.Reader.PeekToken();
			if (nextToken != Token.CmdEnd)
				do
				{
					switch (nextToken)
					{
						case Token.IN:
							code.Reader.ReadToken();
							break;
						case Token.WINDOW:
							code.Reader.ReadToken();
							window = true;
							break;
						default:
							message = Comp.GetCompiledExpression();
							break;
					}
					nextToken = code.Reader.PeekToken();
				} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			String txt;
			if (message == null)
				txt = "Press any key to continue...";
			else
				txt = message.GetString(exec);
			GuineuInstance.WinMgr.Wait(txt, 0, 0, 0, window, true);
		}
	}
}