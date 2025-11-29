using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ToughService.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string emailDestino, string assunto, string mensagemHtml)
        {
            var settings = _configuration.GetSection("EmailSettings");

            string mailServer = settings["MailServer"];
            int mailPort = int.Parse(settings["MailPort"]);
            string senderEmail = settings["SenderEmail"];
            string senderPassword = settings["SenderPassword"];
            string senderName = settings["SenderName"];

            var client = new SmtpClient(mailServer, mailPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = assunto,
                Body = mensagemHtml,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(emailDestino);

            await client.SendMailAsync(mailMessage);
        }
    }
}