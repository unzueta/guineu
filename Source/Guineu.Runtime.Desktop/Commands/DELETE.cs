using System;
using Guineu.Expression;

namespace Guineu.Commands
{
	class DELETE : ICommand
	{
		readonly IteratorClause Iterator;
		readonly InClause In;

		public DELETE()
		{
			Iterator = new IteratorClause();
			In = new InClause();
		}

		public void Compile(CodeBlock code)
		{
			Token nextToken;
			do
			{
				nextToken = code.Reader.ReadToken();
				switch (nextToken)
				{
					case Token.CmdEnd:
						break;
					default:
						if (InClause.Follows(nextToken))
							In.Compile(code);
						else if (IteratorClause.Follows(nextToken))
							Iterator.Compile(nextToken, code);
						else
							throw new ErrorException(ErrorCodes.UnrecognizedKeyword);
						break;
				}
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			using (var csr = In.Get(exec))
			{
				var itr = Iterator.Get(exec);
				while (itr.HasMore(exec))
				{
					csr.Cursor.Delete();
					csr.Cursor.FlushRecord();
					itr.Next(exec);
				}
			}
		}

	}
}