using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class SETSTATUS : ICommand
	{
		Boolean Value;

		public void Compile(CodeBlock code)
		{
			Token nextToken;
			do
			{
				nextToken = code.Reader.ReadToken();
				switch (nextToken)
				{
					case Token.ON:
						Value = true;
						break;
					case Token.OFF:
						Value = false;
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
			GuineuInstance.Set.Status.Value = Value;
		}
	}

	public class SetStatusValue
	{
		private Boolean _Value = true;

		public Boolean Value
		{
			get { return _Value; }

			set { _Value = value; }
		}

	}

}
