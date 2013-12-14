using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class SETBELL : ICommand
	{
		ExpressionBase waveFile;
		Boolean bellOn = true;

		enum Action { OnOff, File };
		Action whatAction;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			Token nextToken;
			do
			{
				nextToken = code.Reader.ReadToken();
				switch (nextToken)
				{
					case Token.ON:
						bellOn = true;
						whatAction = Action.OnOff;
						break;
					case Token.OFF:
						bellOn = false;
						whatAction = Action.OnOff;
						break;
					case Token.TO:
						waveFile = Comp.GetCompiledExpression();
						whatAction = Action.File;
						break;
					case Token.CmdEnd:
						break;
					default:
						// (...) Invalid token
						break;
				}
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			switch (whatAction)
			{
				case Action.OnOff:
					GuineuInstance.Set.Bell = bellOn;
					break;
				case Action.File:
					GuineuInstance.Set.BellFile = waveFile.GetString(context);
					break;
			}
		}

	}

}
