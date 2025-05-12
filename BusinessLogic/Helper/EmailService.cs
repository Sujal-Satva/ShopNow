using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace BusinessLogic.Helper
{
    public class EmailService 
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:Port"]);
            var smtpUsername = _configuration["EmailSettings:Username"];
            var smtpPassword = _configuration["EmailSettings:Password"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var fromName = _configuration["EmailSettings:FromName"];

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
