using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
	class SKIP : ICommand
	{
		ExpressionBase _Alias;
		ExpressionBase _Record;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			Token nextToken = code.Reader.PeekToken();
			while (nextToken != Token.CmdEnd)
			{
				switch (nextToken)
				{
					case Token.IN:
						code.Reader.ReadToken();
						_Alias = Comp.GetCompiledExpression();
						break;
					default:
						_Record = Comp.GetCompiledExpression();
						break;
				}
				nextToken = code.Reader.PeekToken();
			} 
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			ICursor csr;
			if (_Alias == null)
			{
				csr = exec.DataSession.Cursor;
			}
			else
			{
				DataSession ds = exec.DataSession;
				csr = ds[ds.WorkareaFromName(_Alias, exec)];
			}

			Int32 skipBy;
			if (_Record == null)
				skipBy = 1;
			else
				skipBy = _Record.GetInt(exec);

			csr.Skip(skipBy);
		}
	}
}