using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Commands
{
	class DO : ICommand
	{
		ExpressionBase procName;
		ExpressionBase file;
		List<ExpressionBase> parms;

		public void Compile(CodeBlock code)
		{
			parms = new List<ExpressionBase>();
			var comp = new Compiler(null, code);
			procName = comp.GetCompiledExpression();
			Token tok;
			do
			{
				tok = code.Reader.ReadToken();
				switch (tok)
				{
					case Token.IN:
						file = comp.GetCompiledExpression();
						break;
					case Token.WITH:
						CompileWithClause(code, comp);
						break;
				}
			} while (tok != Token.CmdEnd);
		}

		private void CompileWithClause(CodeBlock code, Compiler comp)
		{
			while (true)
			{
				ExpressionBase exp = comp.GetCompiledExpression();
				parms.Add(exp);
				if (code.Reader.PeekToken() == Token.Comma)
					code.Reader.ReadToken();
				else
					break;
			}
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			var name = procName.GetName(context);
			CodeBlock code;
				var resolver = new ProcedureResolver();
				if (file == null)
					resolver.FindProcedure(context, name, out code);
				else
					resolver.FindProcedureIn(
						context,
						file.GetName(context),
						name,
						out code
					);
			if (code == null)
				throw new ErrorException(ErrorCodes.FileNotFound, name + ".PRG");

			var param = ParameterCollection.CreateByReference(context, parms);
			context.Context.ExecuteInNewContext(code, param);
		}
	}
}