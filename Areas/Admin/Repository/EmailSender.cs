using System.Net;
using System.Net.Mail;

namespace Shopping_Tutorial.Areas.Admin.Repository
{
	public class EmailSender : IEmailSender
	{
		public Task SendEmailAsync(string email, string subject, string message)
		{
			var client = new SmtpClient("smtp.gmail.com", 587)
			{
				EnableSsl = true, //bật bảo mật
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential("nguyenhoangthang26032003@gmail.com", "gela zpyi lyto molv")
			};

			return client.SendMailAsync(
				new MailMessage(from: "nguyenhoangthang26032003@gmail.com",
								to: email,
								subject,
								message
								));
		}
	}
}
