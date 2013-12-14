using System;
using System.Collections.Generic;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
	class REPLACE : ICommand
	{
		ExpressionBase alias;
		List<ReplacementEntry> fields;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			fields = new List<ReplacementEntry>();

			Token nextToken = code.Reader.PeekToken();
			ReplacementEntry item = null;
			do
			{
				switch (nextToken)
				{
					case Token.IN:
						code.Reader.ReadToken();
						alias = comp.GetCompiledExpression();
						break;
					case Token.NEXT:
						code.Reader.ReadToken();
						comp.GetCompiledExpression();
						break;
					case Token.WITH:
						code.Reader.ReadToken();
						if (item == null)
							throw new ErrorException(ErrorCodes.Syntax);
						item.Value = comp.GetCompiledExpression();
						fields.Add(item);
						break;
					case Token.Comma:
						code.Reader.ReadToken();
						break;
					default:
						item = new ReplacementEntry { FieldName = comp.GetCompiledExpression() };
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		/// <summary>
		/// Performs the REPLACE statement
		/// </summary>
		/// <param name="exec"></param>
		/// <param name="nextLine"></param>
		/// <remarks>
		/// 
		/// 
		/// Steps:
		///    
		///  1. Obtain locks. REPLACE performs a record lock when used without a clause, or with
		/// the NEXT 1 clause. Any other clause including NEXT 0 performs a table lock
		/// on all tables involved. Locking the record also means that we need to read the 
		/// current value from disk.
		///  
		///  2. Replaces all fields. This happens in the order of fields in the list.
		///  Every field is evaluated at the time it is written. When the same field is
		///  used in subsequent field expressions, it already has the new values.
		///  
		/// 3. Unlock all records.
		/// </remarks>
		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			ICursor csr = exec.GetCursor(alias);

			ICursor csrReplace;
			if (!csr.Eof())
			{
				foreach (ReplacementEntry entry in fields)
				{
					csrReplace = entry.FieldName.GetCursor(exec) ?? csr;
					Nti fieldName = entry.FieldName.ToNti(exec);
					Variant val = entry.Value.GetVariant(exec);
					csrReplace.SetField(fieldName, val);
					csrReplace.FlushRecord();
				}
			}
		}
	}

	class ReplacementEntry
	{
		internal ExpressionBase FieldName;
		internal ExpressionBase Value;
	}
}