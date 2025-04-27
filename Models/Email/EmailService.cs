using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Projekt.Models.Email
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Sklep Spożywczy", _config["SmtpSettings:From"]));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            try
            {
                using var smtp = new SmtpClient();
                _logger.LogInformation("Łączenie z serwerem SMTP...");
                await smtp.ConnectAsync(_config["SmtpSettings:Host"], int.Parse(_config["SmtpSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                _logger.LogInformation("Uwierzytelnianie...");
                await smtp.AuthenticateAsync(_config["SmtpSettings:Username"], _config["SmtpSettings:Password"]);
                _logger.LogInformation("Wysyłanie emaila...");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                _logger.LogInformation("Email wysłany pomyślnie.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wysyłania emaila do {To}", to);
                throw; // Rzucamy wyjątek, aby kontroler mógł go obsłużyć
            }
        }
    }
}