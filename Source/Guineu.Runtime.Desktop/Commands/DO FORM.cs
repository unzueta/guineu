using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu.Commands
{
	class DOFORM : ICommand
	{
		ExpressionBase procName;
		List<ExpressionBase> parms;

		public void Compile(CodeBlock codeBlock)
		{
			parms = new List<ExpressionBase>();
			var comp = new Compiler(null, codeBlock);
			procName = comp.GetCompiledExpression();
			Token tok;
			do
			{
				tok = codeBlock.Reader.ReadToken();
				switch (tok)
				{
					case Token.WITH:
						CompileWith(codeBlock, comp);
						break;
				}
			} while (tok != Token.CmdEnd);
		}

		private void CompileWith(CodeBlock codeBlock, Compiler comp)
		{
			while (true)
			{
				ExpressionBase exp = comp.GetCompiledExpression();
				parms.Add(exp);
				if (codeBlock.Reader.PeekToken() == Token.Comma)
					codeBlock.Reader.ReadToken();
				else
					break;
			}
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			string name = procName.GetName(context);
			if (Path.GetExtension(name).ToUpper(System.Globalization.CultureInfo.InvariantCulture) != ".SCX")
				name = name + ".scx";
			name = name + ".FXP";
			CodeBlock cb;
			var resolver = new ProcedureResolver();
			resolver.FindProcedure(context, name, out cb);

			if (cb == null)
				throw new ErrorException(ErrorCodes.FileNotFound, name);

			var param = new ParameterCollection(context, parms);
			context.Context.ExecuteInNewContext(cb, param);
		}
	}
}