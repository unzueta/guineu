using System;
using Guineu.Expression;

namespace Guineu.Commands
{
	class CLOSEDATABASES : ICommand
	{
		public void Compile(CodeBlock code)
		{
			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.ALL:
						code.Reader.ReadToken();
						break;

					case Token.CmdEnd:
						break;

					default:
						throw new ErrorException(ErrorCodes.UnrecognizedKeyword);
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			exec.DataSession.CloseAll();
			exec.DataSession.Select(1);
		}
	}
}