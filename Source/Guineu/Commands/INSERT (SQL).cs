using System;
using System.Collections.Generic;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
	class INSERTSQL : ICommand
	{
		ExpressionBase alias;
		readonly List<ExpressionBase> fields = new List<ExpressionBase>();
		readonly List<ExpressionBase> values = new List<ExpressionBase>();

		enum Element
		{
			Fields,
			Values
		}

		public void Compile(CodeBlock code)
		{
			Token nextToken = code.Reader.PeekToken();
			var comp = new Compiler(null, code);
			var current = Element.Fields;

			do
			{
				switch (nextToken)
				{
					case Token.INTO:
						code.Reader.ReadToken();
						alias = comp.GetCompiledExpression();
						break;
					case Token.OpenParenthesis:
					case Token.Parenthesis:
					case Token.Comma:
						code.Reader.ReadToken();
						break;
					case Token.VALUES:
						code.Reader.ReadToken();
						current = Element.Values;
						break;
					default:
						List<ExpressionBase> list = (current == Element.Fields ? fields : values); 
						list.Add(comp.GetCompiledExpression());
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			ICursor csr = exec.GetCursor(alias);
			csr.Append();

			for(var i=0;i<values.Count;i++)
			{
				Variant val = values[i].GetVariant(exec);
				Nti fieldName;
				if (fields.Count > i)
					fieldName = fields[i].ToNti(exec);
				else
				{
					fieldName = csr.Columns[i].Name;
				}
				csr.SetField(fieldName, val);
				csr.FlushRecord();
			}
		}
	}
}