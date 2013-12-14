using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Data;
using Guineu.Data.Engines.Dbf;
using Guineu.Expression;

namespace Guineu
{
	class SETORDER : ICommand
	{
		ExpressionBase _Order;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			Token nextToken;
			do
			{
				nextToken = code.Reader.ReadToken();
				switch (nextToken)
				{
					case Token.TO:
						_Order = Comp.GetCompiledExpression();
						break;
					case Token.CmdEnd:
						break;
					default:
						// (...) Invalid token
						break;
				}
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			ICursor csr;
			csr = context.DataSession.Cursor;

			Table tbl = csr as Table;
			if (tbl != null)
			{
				String order = _Order.GetName(context);
				tbl.SetOrder(order);
			}

		}
	}

	public class SetOrderValue
	{
		String _Order;

		public String Value
		{
			get { return _Order; }
			set { _Order = value;  }
		}

	}

}
