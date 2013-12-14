using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Commands
{
	class PRIVATE : ICommand
	{
		readonly List<ExpressionBase> names = new List<ExpressionBase>();

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			var nextToken = Token.Comma;
			do
			{
				switch (nextToken)
				{
					case Token.Comma:
						ExpressionBase var = comp.GetCompiledExpression();
						names.Add(var);
						break;
					default:
						throw new ErrorException(ErrorCodes.Syntax);
				}
				nextToken = code.Reader.ReadToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			foreach (var name in names)
				exec.PrivatePatterns.Add(new Nti(name.GetName(exec)));
		}
	}
}