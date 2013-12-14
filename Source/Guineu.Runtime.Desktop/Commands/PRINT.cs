using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{

	class PRINT : ICommand
	{
		ExpressionBase[] ex;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);

			// Determine the number of expressions
			int Expressions = Comp.ReadInt();
			ex = new ExpressionBase[Expressions];

			// Print all expressions in a new line
			for (int i = 0; i < Expressions; i++)
			{
				ex[i] = Comp.GetCompiledExpression();
				code.Reader.ReadByte();
			}
		}
		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			GuineuInstance.WinMgr.WriteLine();
			for (int i = 0; i < ex.Length; i++)
			{
				String strOut = ex[i].GetString(exec);
				GuineuInstance.WinMgr.Write(strOut);
				if (i < ex.Length - 1)
				{
					GuineuInstance.WinMgr.Write(" ");
				}
			}
		}
	}

}