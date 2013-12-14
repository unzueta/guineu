using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class SETNOTIFY : ICommand
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
			GuineuInstance.Set.Notify.Value = Value;
		}
	}

	public class SetNotifyValue
	{
		private Boolean _Value = true;

		public Boolean Value
		{
			get { return _Value; }

			set { _Value = value; }
		}

	}

}
