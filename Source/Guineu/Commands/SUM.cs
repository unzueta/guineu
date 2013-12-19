using System;
using Guineu.Commands;
using Guineu.Expression;

namespace Guineu
{

	class SUM : ICommand
	{
		readonly IteratorClause iterator;
		readonly InClause In;

		public SUM()
		{
			iterator = new IteratorClause(true);
			In = new InClause();
		}

		ExpressionBase toClause;
		ExpressionBase expr;

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

					case Token.Expression:
						expr = comp.GetCompiledExpression();
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
			// TODO: Use a different type depending on the type
			double total = 0;

			using (var csr = In.Get(context))
			{
				var itr = iterator.Get(context);
				while (itr.HasMore(context))
				{
					total += expr.GetDouble(context);
					itr.Next(context);
				}
			}

			// TODO: Determine the number of decimal places from the field expression
			var result = new VariableAssignment(toClause, new Variant(total, 10, 2));
			result.Do(context);
		}
	}

}
