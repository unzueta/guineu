using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class SETDELETED : ICommand
	{
		Boolean Deleted;

		public void Compile(CodeBlock code)
		{
			Token nextToken;
			do
			{
				nextToken = code.Reader.ReadToken();
				switch (nextToken)
				{
					case Token.ON:
						Deleted = true;
						break;
					case Token.OFF:
						Deleted = false;
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
			 GuineuInstance.Set.Deleted.Value = Deleted;
		}
	}

	// TODO: SET DELETED is scoped to the current datasession
	public class SetDeletedValue
	{
		private Boolean _Deleted;

		public Boolean Value
		{
			get { return _Deleted; }

			set {_Deleted = value; }
		}

	}

}
