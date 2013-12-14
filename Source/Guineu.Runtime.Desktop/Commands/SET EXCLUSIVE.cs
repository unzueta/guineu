using System;
using Guineu.Expression;

namespace Guineu.Commands
{
	class SETEXCLUSIVE : ICommand
	{
		Boolean isExclusive;

		public void Compile(CodeBlock code)
		{
			Token nextToken;
			do
			{
				nextToken = code.Reader.ReadToken();
				switch (nextToken)
				{
					case Token.ON:
						isExclusive = true;
						break;
					case Token.OFF:
						isExclusive = false;
						break;
					case Token.CmdEnd:
						break;
				}
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			GuineuInstance.Set.Exclusive.Value = isExclusive;
		}
	}

	public class SetExclusiveValue
	{
		public bool Value { get; set; }
	}

}
