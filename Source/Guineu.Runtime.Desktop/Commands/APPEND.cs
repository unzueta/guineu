using System;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
	class APPEND : ICommand
	{
		ExpressionBase alias;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);

			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.IN:
						code.Reader.ReadToken();
						alias = comp.GetCompiledExpression();
						break;
					case Token.BLANK:
						code.Reader.ReadToken();
						break;
					default:
						throw new ErrorException(ErrorCodes.UnrecognizedKeyword);
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			ICursor csr = exec.GetCursor(alias);
			csr.Append();
		}
	}
}