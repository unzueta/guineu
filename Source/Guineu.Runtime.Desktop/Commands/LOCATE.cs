using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{

	class LOCATE : ICommand
	{
		ExpressionBase forClause;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.FOR:
						code.Reader.ReadToken();
						forClause = Comp.GetCompiledExpression();
						break;

					default:
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			ICursor csr = context.GetCursor();
			csr.GoToTop();
			while (!csr.Eof())
			{
				if (forClause == null || forClause.GetBool())
					break;
				csr.Skip(1);
			}
			csr.Found = !csr.Eof();
			csr.LocateExpression = forClause;
		}
	}

}
