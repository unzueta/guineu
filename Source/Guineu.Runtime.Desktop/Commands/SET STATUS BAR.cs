using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class SETSTATUSBAR : ICommand
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
			GuineuInstance.Set.StatusBar.Value = Value;
		}
	}

	public class SetStatusBarValue
	{
		private Boolean _Value = true;

		public Boolean Value
		{
			get { return _Value; }

			// TODO: Hide status bar
			set 
			{ 
				_Value = value; 
			}
		}

	}

}
