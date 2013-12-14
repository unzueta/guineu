using System;
using System.IO;
using Guineu.Data.Engines.Dbf;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu
{

	class USE : ICommand
	{
		ExpressionBase table;
		ExpressionBase tag;
		Boolean exclusive;
		ExpressionBase inClause;
		ExpressionBase aliasClause;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);

			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.ORDER:
						code.Reader.ReadToken();
						tag = comp.GetCompiledExpression();
						break;

					case Token.EXCLUSIVE:
						code.Reader.ReadToken();
						exclusive = true;
						break;

					case Token.SHARED:
						code.Reader.ReadToken();
						exclusive = false;
						break;

					case Token.IN:
						code.Reader.ReadToken();
						inClause = comp.GetCompiledExpression();
						break;

					case Token.ALIAS_Clause:
						code.Reader.ReadToken();
						aliasClause = comp.GetCompiledExpression();
						break;

					default:
						if (nextToken != Token.CmdEnd)
							table = comp.GetCompiledExpression();
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			// Check for question mark
			String name = null;
			if (table != null)
			{
				name = table.GetString(exec);
				if (name == "?")
				{
					TableOpenDialogResult dlgResult = GuineuInstance.WinMgr.ShowOpenTableDialog();
					name = dlgResult.TableName;
				}
			}

			// With an empty Name no table is opened or closed. Thas' the case when either
			// the file dialog has been cancelled or the user passed an empty string.
			if (table != null && String.IsNullOrEmpty(name))
				return;

			// Close any existing table in the desired work area
			Int32 area;
			DataSession datasession = exec.DataSession;
			if (inClause == null)
				area = datasession.Select();
			else
			{
				area = exec.DataSession.WorkareaFromName(inClause,exec);
				if (area == 0)
					area = exec.DataSession.GetNextFreeWorkarea();
			}
			exec.DataSession.Close(area);

			if (String.IsNullOrEmpty(name))
				return;


			String tableName = Path.ChangeExtension(name, "DBF");
			tableName = tableName.Replace('\\', Path.DirectorySeparatorChar);

			String alias = datasession.GetAlias(name,area,aliasClause);

			Table result;
			try
			{
				result = new Table(alias, tableName, exclusive);
			}
			catch (FileNotFoundException)
			{
				// (...) Change to a full path
				String fullpath = tableName;
				throw new ErrorException(ErrorCodes.FileNotFound, fullpath);
			}
			catch (IOException io)
			{
				throw new ErrorException(ErrorCodes.FileIsInUse, io.Message);
			}

			if (tag != null)
			{
				String order = tag.GetName(exec);
				result.SetOrder(order);
			}

			exec.DataSession.Add(result, area);
		}
	}

}