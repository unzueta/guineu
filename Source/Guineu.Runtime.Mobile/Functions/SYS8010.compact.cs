using System;
using Guineu.Expression;
using System.Collections.Generic;
using System.Net;

// Code depends on the OpenNet CF FTP class. Removed until dependencies to binary DLL are resolved.

namespace Guineu.Functions
{
	/// <summary>
	/// FTP access
	/// </summary>
	 class SYS8010 : ISys
	{
		public String getString(CallingContext context, List<ExpressionBase> param)
		{
			return "";

			//String url;
			//String passWord;
			//String userName;
			//Byte[] data = null;

			//switch (param.Count)
			//{
			//  case 0:
			//  case 1:
			//  case 2:
			//    throw new ErrorException(ErrorCodes.TooFewArguments);
			//  case 3:
			//  case 4:
			//  case 5:
			//  case 6:
			//    url = param[2].GetString(context);
			//    if (param.Count >= 4)
			//      userName = param[3].GetString(context);
			//    else
			//      userName = null;
			//    if (param.Count >= 5)
			//      passWord = param[4].GetString(context);
			//    else
			//      passWord = "";
			//    if (param.Count >= 6)
			//      data = GuineuInstance.CurrentCp.GetBytes(param[5].GetString(context));
			//    break;
			//  default:
			//    throw new ErrorException(ErrorCodes.TooManyArguments);
			//}

			//var uri = new Uri(url);
			//var rc = new FtpRequestCreator();
			//var req = (FtpWebRequest)rc.Create(uri);
			//req.UsePassive = true;
			//req.UseBinary = true;
			
			//if (!(String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(passWord)))
			//  req.Credentials = new NetworkCredential(userName, passWord);

			//var methodName = param[1].ToNti(context);
			//switch (methodName.ToKnownNti())
			//{
			//  case KnownNti.Get:
			//    return DownloadFile(req);
			//  case KnownNti.Put:
			//    if (data == null)
			//      throw new ErrorException(ErrorCodes.InvalidArgument);
			//    return UploadFile(req, data);
			//  default:
			//    throw new ErrorException(ErrorCodes.InvalidArgument);
			//}
		}

		static string DownloadFile(WebRequest req)
		{
			return "";

			//req.Method = WebRequestMethods.Ftp.DownloadFile;
			//try
			//{
			//  using (var res = (FtpWebResponse)req.GetResponse())
			//  {
			//    var st = res.GetResponseStream();
			//    const int bufferSize = 256;
			//    var buffer = new byte[bufferSize + 1];
			//    var retVal = "";
			//    while (true)
			//    {
			//      int bytesRead = st.Read(buffer, 0, bufferSize);
			//      retVal += GuineuInstance.CurrentCp.GetString(buffer, 0, bytesRead);
			//      if (bytesRead == 0)
			//        return retVal;
			//    }
			//  }
			//}
			//catch
			//{
			//  return null;
			//}
		}

		static string UploadFile(WebRequest req, byte[] data)
		{
			return "";

			//req.Method = WebRequestMethods.Ftp.UploadFile;
			//try
			//{
			//  using (var s = req.GetRequestStream())
			//    s.Write(data, 0, data.Length);
			//  return data.Length.ToString();
			//}
			//catch
			//{
			//  return "0";
			//}
		}
	}
}