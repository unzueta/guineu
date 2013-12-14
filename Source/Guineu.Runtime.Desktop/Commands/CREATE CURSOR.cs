using System;
using System.Collections.Generic;
using System.Data;
using Guineu.Data.Engines.Cursor;
using Guineu.Expression;

namespace Guineu
{
	class CREATECURSOR : ICommand
	{
		ExpressionBase alias;
		List<CursorFieldEntry> fields;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			alias = comp.GetCompiledExpression();
			fields = new List<CursorFieldEntry>();

			Token nextToken = code.Reader.ReadToken();
			do
			{
				switch (nextToken)
				{
					case Token.OpenParenthesis:
						ReadStructure(code, comp);
					// case Token.CODEPAGE:
						// (...) 
						break;
					default:
						// (...) Invalid token
						break;
				}
				nextToken = code.Reader.ReadToken();
			} while (nextToken != Token.CmdEnd);
		}

		private void ReadStructure(CodeBlock code, Compiler comp)
		{
			Token nextToken = Token.Comma;
			CursorFieldEntry entry = null;
			do
			{
				switch (nextToken)
				{
					case Token.Comma:
						entry = new CursorFieldEntry();
						fields.Add(entry);
						entry.FieldName = comp.GetCompiledExpression();
						entry.TypeDef = comp.GetCompiledExpression();
						break;

					case Token.OpenParenthesis:
						if (entry != null)
						{
							entry.Size = comp.GetCompiledExpression();
							if (code.Reader.ReadToken() == Token.Comma)
							{
								entry.Decimals = comp.GetCompiledExpression();
								code.Reader.ReadToken();
							}
						}
						break;
				}
				nextToken = code.Reader.ReadToken();
			} while (nextToken != Token.Parenthesis);
			
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			var tbl = new DataTable();

			foreach (CursorFieldEntry entry in fields)
			{
				String fieldName = entry.FieldName.GetName(exec);
				String fieldType = entry.TypeDef.GetName(exec);
				Int32 length;
				if( entry.Size == null )
					length = 0;
				else
					length = entry.Size.GetInt(exec);
				AddColumn(tbl, fieldName, fieldType, length);
			}

			var area = exec.DataSession.GetWorkArea(alias, exec);
			if(area != 0)
				exec.DataSession.Close(area);

			String a = alias.GetString(exec);
			var result = new Cursor(a, tbl);
			exec.DataSession.Add(result, exec.DataSession.GetNextFreeWorkarea());
		}

		private static void AddColumn(DataTable tbl, string fieldName, string fieldType, int length)
		{
			var col = new DataColumn(fieldName);
			Char dataType = fieldType[0];
			switch (dataType)
			{
				case 'm':
				case 'M':
					col.DataType = typeof(String);
					break;
				case 'c':
				case 'C':
					col.DataType = typeof(String);
					col.MaxLength = length;
					break;
				case 'n':
				case 'N':
					col.DataType = typeof(Decimal);
					break;
				case 'i':
				case 'I':
					col.DataType = typeof(Int32);
					break;
				case 't':
				case 'T':
				case 'd':
				case 'D':
					col.DataType = typeof (DateTime);
					break;
				case 'l':
				case 'L':
					col.DataType = typeof (Boolean);
					break;
				default:
					break;
			}
			tbl.Columns.Add(col);
		}
}

	class CursorFieldEntry
	{
		internal ExpressionBase FieldName;
		internal ExpressionBase TypeDef;
		internal ExpressionBase Size;
		internal ExpressionBase Decimals;
	}
}