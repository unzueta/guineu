using Guineu.Data;
using Guineu.Data.Engines.Dbf;
using Guineu.Expression;

namespace Guineu.Commands
{
	class SETFILTER : ICommand
	{
		ExpressionBase filter;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			Token nextToken;
			do
			{
				nextToken = code.Reader.ReadToken();
				switch (nextToken)
				{
					case Token.TO:
						filter = comp.GetCompiledExpression();
						if (filter == null)
							nextToken = Token.CmdEnd;
						break;
					case Token.CmdEnd:
						break;
				}
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			ICursor csr = context.DataSession.Cursor;

			var tbl = csr as Table;
			if (tbl != null)
				tbl.SetFilter(filter);
		}
	}
}
