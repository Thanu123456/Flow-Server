using Flow_Api.Services.Interfaces.Notifications;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Flow_Api.Services.Implementations.Notifications
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string ownerName, string shopName)
        {
            var subject = "Welcome to POS System - Registration Submitted";
            var body = $@"
                <h2>Welcome {ownerName}!</h2>
                <p>Thank you for registering {shopName} with our POS system.</p>
                <p>Your account is currently pending approval from our admin team.</p>
                <p>You will receive an email notification once your account is approved.</p>
                <p>This typically takes 24-48 hours.</p>
                <br>
                <p>Best regards,<br>POS System Team</p>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendApprovalEmailAsync(string toEmail, string ownerName, string shopName, string loginUrl)
        {
            var subject = "Your POS Account Has Been Approved!";
            var body = $@"
                <h2>Congratulations {ownerName}!</h2>
                <p>Your account for {shopName} has been approved.</p>
                <p>You can now login to your POS system using your credentials.</p>
                <p><a href='{loginUrl}'>Click here to login</a></p>
                <br>
                <p>Best regards,<br>POS System Team</p>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendRejectionEmailAsync(string toEmail, string ownerName, string reason)
        {
            var subject = "POS Account Registration Update";
            var body = $@"
                <h2>Hello {ownerName},</h2>
                <p>We regret to inform you that your registration was not approved.</p>
                <p><strong>Reason:</strong> {reason}</p>
                <p>If you have any questions, please contact our support team.</p>
                <br>
                <p>Best regards,<br>POS System Team</p>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var subject = "Password Reset Request";
            var body = $@"
                <h2>Password Reset</h2>
                <p>You have requested to reset your password.</p>
                <p><a href='{resetLink}'>Click here to reset your password</a></p>
                <p>This link will expire in 1 hour.</p>
                <p>If you didn't request this, please ignore this email.</p>
                <br>
                <p>Best regards,<br>POS System Team</p>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["Email:SmtpUsername"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail ?? "noreply@posystem.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
