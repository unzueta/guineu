using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Commands
{
	class PUBLIC : ICommand
	{
		readonly List<ExpressionBase> names;

		public PUBLIC()
		{
			names = new List<ExpressionBase>();
		}

		/// <summary>
		/// PUBLIC declares global variables. During compilation we determine the names
		/// of all variables. Names don't have to be hardcoded. Any expression that
		/// returns a string can be used as a variable Name:
		/// 
		/// PUBLIC lcName, (SYS(2015)), (m.lcVarName)
		/// </summary>
		/// <param name="code"></param>
		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			Token nextToken = Token.Comma;
			do
			{
				switch (nextToken)
				{
					case Token.Comma:
						ExpressionBase var = comp.GetCompiledExpression();
						names.Add(var);
						break;
					case Token.AS:
						// (...) Skip type, etc.
						break;
					default:
						// (...) Invalid token
						break;
				}
				nextToken = code.Reader.ReadToken();
			} while (nextToken != Token.CmdEnd);
		}

	    /// <summary>
	    /// When we execute the PUBLIC statement, we need to create missing
	    /// variables.
	    /// </summary>
	    /// <param name="exec"></param>
	    /// <param name="nextLine"></param>
	    /// <returns></returns>
	    public void Do(CallingContext exec, ref Int32 nextLine)
		{
			for (var curName = 0; curName < names.Count; curName++)
				GuineuInstance.Public.AddVariable(exec, names[curName], null);
		}
	}
}