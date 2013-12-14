using System;
using Guineu.Expression;

namespace Guineu
{
	class DELETEFILE : ICommand
	{
		ExpressionBase FileName;
		Boolean Recycle;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.RECYCLE:
						code.Reader.ReadToken();
						Recycle = true;
						break;

					default:
						FileName = comp.GetCompiledExpression();
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}


		public void Do(CallingContext context, ref Int32 nextLine)
		{
			var name = FileName.GetString(context);
			ERASE.EraseFile(name, Recycle);
		}
	}
}