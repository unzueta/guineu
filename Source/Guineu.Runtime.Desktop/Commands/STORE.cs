using System;
using System.Collections.Generic;
using Guineu.Commands;
using Guineu.Expression;

namespace Guineu
{

	class STORE : ICommand
	{
		List<VariableAssignment> vars;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			ExpressionBase expr = comp.GetCompiledExpression();

			Token nextToken = code.Reader.PeekToken();
			vars = new List<VariableAssignment>();
			do
			{
				switch (nextToken)
				{
					case Token.TO:
					case Token.Comma:
						code.Reader.ReadToken();
						break;
					default:
						ExpressionBase var = comp.GetCompiledExpression();
						vars.Add(new VariableAssignment(var, expr));
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			foreach (VariableAssignment var in vars)
				var.Do(context);
		}
	}
}
