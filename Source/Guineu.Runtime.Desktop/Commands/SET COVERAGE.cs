using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class SETCOVERAGE : ICommand
	{
		ExpressionBase file;
		Boolean additive;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			Token nextToken;
			do
			{
				nextToken = code.Reader.PeekToken();
				switch (nextToken)
				{
					case Token.TO:
						code.Reader.ReadToken();
						break;

					case Token.ADDITIVE:
						code.Reader.ReadToken();
						additive = true;
						break;

					case Token.CmdEnd:
						break;

					default:
						if (file == null)
							file = Comp.GetCompiledExpression();
						break;
				}
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			if (GuineuInstance.ProfileStream != null)
				GuineuInstance.ProfileStream.Close();
			if (file == null)
				GuineuInstance.ProfileStream = null;
			else
			{
				String name = file.GetString(context);
				FileMode fm;
				if (additive)
					fm = FileMode.Append;
				else
					fm = FileMode.Create;
				GuineuInstance.ProfileStream = new FileStream(name, fm);
			}
		}
	}
}
