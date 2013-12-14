using System;
using Guineu.Expression;

namespace Guineu.Commands
{
	class EXTERNAL : ICommand
	{
		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.ARRAY:
						code.Reader.ReadToken();
						comp.GetCompiledExpression();
						break;
					default:
						throw new ErrorException(ErrorCodes.Syntax);
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)	{}
	}
}