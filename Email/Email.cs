using Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Email
{
    public class CustomEmailSender : IEmail
    {
        private readonly AppSettings _appSettings;

        public CustomEmailSender(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task SendAsync(string from, string to, string subject, string html)
        {
            var fromAddress = new MailAddress(string.IsNullOrEmpty(from) ? "aloisio.developer@gmail.com" : from, ".NET Auth");
            var toAddress = new MailAddress(to, to);

            using var email = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = html,
                IsBodyHtml = true,
            };

            await SendAsync(email);
        }
        public async Task SendAsync(string from, IEnumerable<string> to, string subject, string html)
        {
            var fromAddress = new MailAddress(string.IsNullOrEmpty(from) ? _appSettings.SMTP.From : from, ".NET Auth");

            using var email = new MailMessage()
            {
                From = fromAddress,
                Subject = subject,
                Body = html,
                IsBodyHtml = true,
            };

            foreach (var recipient in to)
            {
                email.To.Add(recipient);
            }

            await SendAsync(email);
        }

        public async Task SendAsync(MailMessage email)
        {
            try
            {
                // Send email
                using var smtp = new SmtpClient(_appSettings.SMTP.Host, _appSettings.SMTP.Port)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(email.From.Address, _appSettings.SMTP.Password),
                    Timeout = 20000,
                };

                await smtp.SendMailAsync(email);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
