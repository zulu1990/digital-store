using Application.Common.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Common.Services
{
    public class GmailSender : IEmailSender
    {
        private readonly SMTPSettings _smtpSettings;

        public GmailSender(IOptions<SMTPSettings> settings)
        {
            _smtpSettings = settings.Value;   
        }

        public async Task SendEmailAsync(string email, string subject, string body, bool isHtml = false)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password),
                EnableSsl = true,
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Email),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendMailVeryfication(string email, Guid verificatioCode)
        {
            string linkLocation = $"{_smtpSettings.VerificationAddress}?email={email}&verificationCode={verificatioCode}";
            var messageBody = $"Please Follow this link to verify email <a href=\"{linkLocation}\">Verify Email</a>";

            await SendEmailAsync(email, "Please Confirm Email", messageBody, true);
        }

        public async Task SendPasswordResetMail(string email, Guid verificatioCode)
        {
            string linkLocation = $"{_smtpSettings.PasswordResetAddress}?verificationCode={verificatioCode}";
            string messageBody = $"Please follow this link to Reset Password <a href=\"{linkLocation}\">Reset Password</a>";

            await SendEmailAsync(email, "Reset Password", messageBody, true);
        }
    }
}
