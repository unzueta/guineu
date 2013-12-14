using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Commands
{
	class LPARAMETERS : ICommand
	{
		readonly List<ExpressionBase> names = new List<ExpressionBase>();

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			Token nextToken = Token.Comma;
			do
			{
				switch (nextToken)
				{
					case Token.Comma:
						ExpressionBase var = comp.GetCompiledExpression();
						names.Add(var);
						break;
					case Token.AS:
						// (...) Skip type, etc.
						break;
					default:
						// (...) Invalid token
						break;
				}
				nextToken = code.Reader.ReadToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			var param = exec.Stack.Parameters;
			if (param.Count > names.Count)
				throw new ErrorException(ErrorCodes.MustSpecifyAdditionalParameters);

			for (var curName = 0; curName < names.Count; curName++)
			{
				ValueMember val = null;
				if (curName < param.Count)
					val = param[curName];
				exec.Locals.AddVariable(exec, names[curName], val);
			}
		}

	}
}