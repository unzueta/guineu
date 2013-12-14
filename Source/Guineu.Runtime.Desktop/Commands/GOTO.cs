using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
	class GOTO : ICommand
	{
		ExpressionBase _Alias;
		Clauses _Clauses;
		ExpressionBase _Record;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			_Clauses = Clauses.Record;
			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.IN:
						code.Reader.ReadToken();
						_Alias = Comp.GetCompiledExpression();
						break;
					case Token.TOP:
						code.Reader.ReadToken();
						_Clauses = Clauses.Top;
						break;
					case Token.BOTTOM:
						code.Reader.ReadToken();
						_Clauses = Clauses.Bottom;
						break;
					default:
						_Record = Comp.GetCompiledExpression();
						_Clauses = Clauses.Record;
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
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

			// Navigate to the desired record
			switch (_Clauses)
			{
				case Clauses.Top:
					csr.GoToTop();
					break;
				case Clauses.Bottom:
					csr.GoToBottom();
					break;
				case Clauses.Record:
					csr.GoTo(_Record.GetInt(exec));
					break;
				default:
					break;
			}

		}

		enum Clauses
		{
			Top,
			Bottom,
			Record
		}
	}
}