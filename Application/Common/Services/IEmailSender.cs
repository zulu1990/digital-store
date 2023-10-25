namespace Application.Common.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string body, bool isHtml= false);

        Task SendMailVeryfication(string email, Guid verificatioCode);

        Task SendPasswordResetMail(string email, Guid verificatioCode);
    }
}
