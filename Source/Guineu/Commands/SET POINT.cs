using System;
using System.Globalization;
using Guineu.Expression;

namespace Guineu.Commands
{
	class SETPOINT : ICommand
	{
		ExpressionBase value;

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
						value = comp.GetCompiledExpression();
						if (value == null)
							nextToken = Token.CmdEnd;
						break;
					case Token.CmdEnd:
						break;
				}
			} while (nextToken != Token.CmdEnd);
		}


		public void Do(CallingContext context, ref int nextLine)
		{
			if (value == null)
				GuineuInstance.Set.Point.Value = ".";
			else
				GuineuInstance.Set.Point.Value = value.GetString(context);
		}
	}

	public class SetPointValue
	{
		public SetPointValue()
		{
			Value = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
		}
		public String Value { get; set; }
	}

}
