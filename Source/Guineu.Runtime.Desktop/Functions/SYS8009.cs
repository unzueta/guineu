using System;
using Guineu.Expression;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace Guineu.Functions
{
	/// <summary>
	/// HTTP access
	/// </summary>
    class SYS8009 : ISys
	{
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			String url;
			String method;
			Byte[] data = null;

			switch (param.Count)
			{
				case 0:
				case 1:
				case 2:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 3:
					method = param[1].GetString(context);
					url = param[2].GetString(context);
					break;
				case 4:
					method = param[1].GetString(context);
					url = param[2].GetString(context);
					data = GuineuInstance.CurrentCp.GetBytes(param[3].GetString(context));
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}

			HttpWebRequest req;
			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
			}
			catch (Exception) 
			{
				return null;
			}
			req.ContentType = "application/x-www-form-urlencoded";
			req.Method = method;

			if (data != null)
			{
				req.ContentLength = data.Length;
				try
				{
					using (var s = req.GetRequestStream())
						s.Write(data, 0, data.Length);
				}
				catch
				{
					return null;
				}
			}

			try
			{
				using (var res = (HttpWebResponse)req.GetResponse())
					using (var sr = new StreamReader(res.GetResponseStream(), GuineuInstance.CurrentCp))
						return sr.ReadToEnd();
			}
			catch
			{
				return null;
			}
		}

	}

}