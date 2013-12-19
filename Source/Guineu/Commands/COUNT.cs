using System;
using Guineu.Commands;
using Guineu.Expression;

namespace Guineu
{

	class COUNT : ICommand
	{
		readonly IteratorClause iterator;
		readonly InClause In;

		public COUNT()
		{
			iterator = new IteratorClause(true);
			In = new InClause();
		}

		ExpressionBase toClause;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);

			Token nextToken;
			do
			{
				nextToken = code.Reader.PeekToken();
				switch (nextToken)
				{
					case Token.TO:
						code.Reader.ReadToken();
						toClause = comp.GetCompiledExpression();
						break;


					case Token.CmdEnd:
						break;

					default:
						code.Reader.ReadToken();
						if (InClause.Follows(nextToken))
							In.Compile(code);
						else if (IteratorClause.Follows(nextToken))
							iterator.Compile(nextToken, code);
						else
							throw new ErrorException(ErrorCodes.UnrecognizedKeyword);
						break;
				}
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			var cnt = 0;

			using (var csr = In.Get(context))
			{
				var itr = iterator.Get(context);
				while (itr.HasMore(context))
				{
					cnt++;
					itr.Next(context);
				}
			}
			
			var result = new VariableAssignment(toClause, new Variant(cnt, 10));
			result.Do(context);
		}
	}

}
