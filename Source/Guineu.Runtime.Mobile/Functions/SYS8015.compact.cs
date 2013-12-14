using System;
using System.Text;
using OpenNETCF.Net.Mail;

namespace Guineu.Functions
{
	/// <summary>
	/// Set time
	/// </summary>
	public partial class SYS8015
	{
		 partial void SendMail(string from, string to, string server, String user, string password, string body, string subject)
		{
			var client = new SmtpClient(server);
			var mailMessage = new MailMessage(from, to);

			var credential = new SmtpCredential(user, password, "localhost");
			mailMessage.Body = body;
			mailMessage.Subject = subject;

			mailMessage.BodyEncoding = Encoding.ASCII;
			mailMessage.SubjectEncoding = Encoding.ASCII;
			client.Credentials = credential;

			client.DeliveryMethod = SmtpDeliveryMethod.Network;

			try
			{
				client.Send(mailMessage);
			}
			catch
			{

			}
		}
	}
}