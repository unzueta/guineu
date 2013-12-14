using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Guineu.Data;
using Guineu.Data.Engines.Dbf;
using Guineu.Expression;

namespace Guineu
{
	class SEEK : ICommand
	{
		ExpressionBase _Value;
		ExpressionBase _Alias;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);

			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.IN:
						code.Reader.ReadToken();
						_Alias = Comp.GetCompiledExpression();
						break;
					default:
						_Value = Comp.GetCompiledExpression();
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			ICursor csr = exec.GetCursor(_Alias);
			Table tbl = csr as Table;
			if (tbl != null)
				tbl.Seek(_Value.GetVariant(exec));

		}


	}
}