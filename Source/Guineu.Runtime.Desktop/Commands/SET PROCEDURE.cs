using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu.Commands
{
	class SETPROCEDURE : ICommand
	{
		ExpressionBase procedure;
		Boolean additive;

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
						procedure = comp.GetCompiledExpression();
						break;
					case Token.ADDITIVE:
						additive = true;
						break;
					case Token.CmdEnd:
						break;
				}
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			if (additive)
				GuineuInstance.Set.Procedure.Add(procedure.GetString(context));
			else
				GuineuInstance.Set.Procedure.Value = procedure.GetString(context);
		}
	}

	public class SetProcedureValue : IEnumerable<CompiledProgram>
	{
		String procedure;
		readonly List<CompiledProgram> procedureList = new List<CompiledProgram>();

		public String Value
		{
			get { return procedure; }
			set { procedure = value; LoadProcedures(); }
		}

		void LoadProcedures()
		{
			// TODO: Handle all variations of SET PROCEDURE TO
			procedureList.Clear();
			foreach (var s in procedure.Split(','))
			{
				var name = Path.ChangeExtension(s, "FXP");
				procedureList.Add(new CompiledProgram(name));
			}
		}

		public IEnumerator<CompiledProgram> GetEnumerator()
		{
			return procedureList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return procedureList.GetEnumerator();
		}

		public void Add(string s)
		{
			var filename = Path.ChangeExtension(s, "FXP");
			// TODO: Refactoring. String handling isn't optimal here.
			var fullName = GuineuInstance.FileMgr.FullPath(filename, false).ToUpper(System.Globalization.CultureInfo.InvariantCulture);
			foreach (var prg in GuineuInstance.Set.Procedure)
			{
				if (prg.FullName == fullName)
					return;
			}

			procedureList.Add(new CompiledProgram(fullName));
			procedure += ", " + fullName;
		}
	}
}