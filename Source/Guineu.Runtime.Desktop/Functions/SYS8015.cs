using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	/// <summary>
	/// Set time
	/// </summary>
	partial class SYS8015 : ISys
	{
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
	
			switch (param.Count)
			{
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 8:
					String server = param[3].GetString(context);
					String from = param[1].GetString(context);
					String to = param[2].GetString(context);
					String user = param[4].GetString(context);
					String password = param[5].GetString(context);
					String subject = param[6].GetString(context);
					String body = param[7].GetString(context);
					SendMail(from, to, server, user, password, body, subject);
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			return "";
		}

		partial void SendMail(String from, String to, String server, String user, String password, String subject, String body);
	}
}