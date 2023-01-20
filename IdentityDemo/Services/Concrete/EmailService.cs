using IdentityDemo.Models;
using IdentityDemo.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IdentityDemo.Services.Concrete
{
    public class EmailService : IEmailService
    {
        private readonly IdentityUser<int> _adminInfo;
        private readonly SmtpConfiguration _smtpConfiguration;

        public EmailService(
            IOptions<SmtpConfiguration> smtpConfiguration,
            IOptions<IdentityUser<int>> adminInfo)
        {
            _smtpConfiguration = smtpConfiguration.Value;
            _adminInfo = adminInfo.Value;
        }


        public async Task SendResetPasswordTokenAsync(string username, string email, string token)
        {
            var to = new MailboxAddress(username, email);
            var subject = "Reset password";
            var body = $"<h3>Hello, {username}</h3>\n <p>click this " +
                $"<a href=\"{_smtpConfiguration.BaseUrl}/Identity/ResetPassword?token={token}\">link</a> " +
                $"to reset your password.</p>";
            await SendEmailAsync(to, subject, body);
        }

        private async Task SendEmailAsync(MailboxAddress to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_adminInfo.UserName, _adminInfo.Email));
            email.To.Add(to);
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(_smtpConfiguration.Host, _smtpConfiguration.Port, _smtpConfiguration.UseSsl);
                await smtp.AuthenticateAsync(_smtpConfiguration.Username, _smtpConfiguration.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
