using System;
using System.Collections.Generic;
using Guineu.Data;
using Guineu.Expression;

namespace Guineu
{
	class INSERTSQL : ICommand
	{
		ExpressionBase Alias;
		List<ExpressionBase> Fields = new List<ExpressionBase>();
		List<ExpressionBase> Values = new List<ExpressionBase>();

		enum Element
		{
			Fields,
			Values
		}

		public void Compile(CodeBlock code)
		{
			Token nextToken = code.Reader.PeekToken();
			var comp = new Compiler(null, code);
			Element current = Element.Fields;

			do
			{
				switch (nextToken)
				{
					case Token.INTO:
						code.Reader.ReadToken();
						Alias = comp.GetCompiledExpression();
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
						List<ExpressionBase> list = (current == Element.Fields ? Fields : Values); 
						list.Add(comp.GetCompiledExpression());
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			ICursor csr = exec.GetCursor(Alias);
			csr.Append();

			for(var i=0;i<Values.Count;i++)
			{
				Variant val = Values[i].GetVariant(exec);
				Nti fieldName;
				if (Fields.Count > i)
					fieldName = Fields[i].ToNti(exec);
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