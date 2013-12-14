using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class SETSYSFORMATS : ICommand
	{
		Boolean IsSysformats;

		public void Compile(CodeBlock code)
		{
			Token nextToken;
			do
			{
				nextToken = code.Reader.ReadToken();
				switch (nextToken)
				{
					case Token.ON:
						IsSysformats = true;
						break;
					case Token.OFF:
						IsSysformats = false;
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
			GuineuInstance.Set.Sysformats.Value = IsSysformats;
		}
	}

	// TODO: SET SYSFORMATS is scoped to the current datasession
	public class SetSysformatsValue
	{
		private Boolean _Sysformats;

		public Boolean Value
		{
			get { return _Sysformats; }
			
			// Use system settings
			set 
			{ 
				_Sysformats = value; 
			}
		}

	}

}
