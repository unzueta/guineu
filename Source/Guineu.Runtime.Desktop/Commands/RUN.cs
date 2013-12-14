using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Guineu.Expression;

namespace Guineu
{
	class RUN : ICommand
	{
		ExpressionBase cmdLine;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			cmdLine = Comp.GetCompiledExpression();
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			String cmd = cmdLine.GetString(context).Trim();

			// Check for /N (=nowait) flag
			Boolean waitForExit = true;
			if (cmd.Substring(0,2) == "/N" || cmd.Substring(0,2) == "/n")
			{
				waitForExit = false;
				cmd = cmd.Substring(2).Trim();
			}

			// parse out process Name and arguments
			String cmdName;
			String cmdArg;
					Int32 sepPos = cmd.IndexOf(' ');
			if (sepPos < 0 || sepPos==cmd.Length-1)
			{
				cmdName = cmd;
				cmdArg = "";
			}
			else
			{
				if (cmd[0] == '"')
				{
					Int32 pos2 = cmd.IndexOf('"', 1);
					if (pos2 <= 0)
					{
						cmdName = cmd;
						cmdArg = "";
					}
					else
					{
						cmdName = cmd.Substring(1, pos2 - 1);
						if (pos2 == cmd.Length - 1)
							cmdArg = "";
						else
							cmdArg = cmd.Substring(pos2 + 1);
					}
				}
				else
				{
					cmdName = cmd.Substring(0, sepPos);
					cmdArg = cmd.Substring(sepPos + 1);
				}
			}

			Process prc = Process.Start(cmdName, cmdArg);
			if (waitForExit)
				prc.WaitForExit();
		}
	}
}