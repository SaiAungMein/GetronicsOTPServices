using System.Net.Mail;
using System.Net;
using GetronicsOTPServices.Interfaces;
using GetronicsOTPServices.Configs;
using log4net;

namespace GetronicsOTPServices.CommonServices
{
    public class EmailService: IEmailService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(EmailService));

        private readonly EmailConfig _emailConfig;
        public EmailService(EmailConfig emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public bool Send(string toEmail, string subject, string body)
        {
            string[] receiverEmails = toEmail.Split(',');

            try
            {
                MailMessage msg = new MailMessage();
                string sender = _emailConfig.Email;
                string password = _emailConfig.Password;
                msg.From = new MailAddress(sender);

                foreach (var receiver in receiverEmails)
                {
                    msg.To.Add(receiver);
                }

                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = _emailConfig.Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                NetworkCredential NetworkCred = new NetworkCredential(sender, password);
                smtp.Credentials = NetworkCred;
                smtp.Port = int.Parse(_emailConfig.Port);
                smtp.Send(msg);

                _log.Info($"Successfully sent email to {toEmail}");

                return true;
            }
            catch (Exception e)
            {
                _log.Error(e);

                return false;
            }
        }
    }
}
