using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Commands;
using Guineu.Expression;

namespace Guineu
{

	class STORE : ICommand
	{
		List<VariableAssignment> _Var;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			ExpressionBase expr = Comp.GetCompiledExpression();

			Token nextToken = code.Reader.PeekToken();
			_Var = new List<VariableAssignment>();
			do
			{
				switch (nextToken)
				{
					case Token.TO:
					case Token.Comma:
						code.Reader.ReadToken();
						break;
					default:
						ExpressionBase var = Comp.GetCompiledExpression();
						_Var.Add(new VariableAssignment(var, expr));
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			foreach (VariableAssignment var in _Var)
				var.Do(context);
		}
	}
}
