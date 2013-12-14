using System;
using System.Collections.Generic;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu.Commands
{
	class DIMENSION : ICommand
	{
		readonly List<ExpressionBase> names;

		public DIMENSION()
		{
			names = new List<ExpressionBase>();
		}

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			var nextToken = Token.Comma;
			do
			{
				switch (nextToken)
				{
					case Token.Comma:
						ExpressionBase var = comp.GetCompiledExpression();
						names.Add(var);
						break;
				}
				nextToken = code.Reader.ReadToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			// TODO: During a class definition, we need to create
			//       an array in exec.Write. In regular code, we
			//       have to check for existing LOCALs and create
			//       PRIVATE if needed.
			IMemberList locals = exec.Write;
			int curName;
			for (curName = 0; curName < names.Count; curName++)
			{
				string name = names[curName].GetName(exec);
				var arr = names[curName] as ArrayDefinition;
				if (arr == null)
					throw new ErrorException(ErrorCodes.NotAnArray, name);
				
				var nti = new Nti(name);
				var mbr = arr.GetArrayMember(exec);
				if (mbr == null)
					locals.Add(nti, arr.CreateMember(exec));
				else
					arr.ResizeMember(mbr, exec);
			}
		}
	}
}