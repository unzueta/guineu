using System;
using Guineu.Expression;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace Guineu.Functions
{
	/// <summary>
	/// FTP access
	/// </summary>
	class SYS8010 : ISys
	{
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			String url;
			String passWord;
			String userName;
			Byte[] data = null;

			switch (param.Count)
			{
				case 0:
				case 1:
				case 2:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 3:
				case 4:
				case 5:
				case 6:
					url = param[2].GetString(context);
					userName = param.Count >= 4 ? param[3].GetString(context) : null;
					passWord = param.Count >= 5 ? param[4].GetString(context) : "";
					if (param.Count >= 6)
						data = GuineuInstance.CurrentCp.GetBytes(param[5].GetString(context));
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}

			var req = (FtpWebRequest)FtpWebRequest.Create(url);
			req.UsePassive = true;
			req.UseBinary = true;
			req.KeepAlive = false;

			if (!(String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(passWord)))
				req.Credentials = new NetworkCredential(userName, passWord);

			var methodName = param[1].ToNti(context);
			switch (methodName.ToKnownNti())
			{
				case KnownNti.Get:
					return DownloadFile(req);
				case KnownNti.Put:
					if (data == null)
						throw new ErrorException(ErrorCodes.InvalidArgument);
					return UploadFile(req, data);
				default:
					throw new ErrorException(ErrorCodes.InvalidArgument);
			}
		}

		static string DownloadFile(WebRequest req)
		{
			req.Method = WebRequestMethods.Ftp.DownloadFile;
			try
			{
				using (var res = (FtpWebResponse)req.GetResponse())
				using (var sr = new StreamReader(res.GetResponseStream()))
				{
					String retVal = sr.ReadToEnd();
					return retVal;
				}
			}
			catch
			{
				return null;
			}
		}

		static string UploadFile(WebRequest req, byte[] data)
		{
			req.Method = WebRequestMethods.Ftp.UploadFile;
			try
			{
				using (var s = req.GetRequestStream())
					s.Write(data, 0, data.Length);
				return "";
			}
			catch
			{
				return null;
			}
		}
	}
}