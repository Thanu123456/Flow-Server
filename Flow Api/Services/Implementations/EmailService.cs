using Flow_Api.Configuration;
using Flow_Api.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Flow_Api.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                    EnableSsl = _emailSettings.EnableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent to {to} with subject '{subject}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to} with subject '{subject}'");
                throw;
            }
        }

        public async Task SendOtpEmailAsync(string email, string otp, int otpType)
        {
            string subject;
            string body;

            switch (otpType)
            {
                case 1: // Email Verification
                    subject = "Verify Your Email Address";
                    body = $@"
                        <html>
                        <body>
                            <h2>Email Verification</h2>
                            <p>Thank you for registering with our POS system. Please use the following OTP to verify your email address:</p>
                            <h3 style=""color: #4CAF50;"">{otp}</h3>
                            <p>This OTP will expire in 15 minutes.</p>
                            <p>If you did not request this verification, please ignore this email.</p>
                            <p>Thank you,<br>The POS System Team</p>
                        </body>
                        </html>
                    ";
                    break;
                case 2: // Password Reset
                    subject = "Reset Your Password";
                    body = $@"
                        <html>
                        <body>
                            <h2>Password Reset</h2>
                            <p>We received a request to reset your password. Please use the following OTP to reset your password:</p>
                            <h3 style=""color: #4CAF50;"">{otp}</h3>
                            <p>This OTP will expire in 15 minutes.</p>
                            <p>If you did not request a password reset, please ignore this email.</p>
                            <p>Thank you,<br>The POS System Team</p>
                        </body>
                        </html>
                    ";
                    break;
                case 3: // 2FA
                    subject = "Your One-Time Password (OTP)";
                    body = $@"
                        <html>
                        <body>
                            <h2>One-Time Password (OTP)</h2>
                            <p>Please use the following OTP to complete your login:</p>
                            <h3 style=""color: #4CAF50;"">{otp}</h3>
                            <p>This OTP will expire in 5 minutes.</p>
                            <p>If you did not request this OTP, please ignore this email.</p>
                            <p>Thank you,<br>The POS System Team</p>
                        </body>
                        </html>
                    ";
                    break;
                default:
                    subject = "Your One-Time Password (OTP)";
                    body = $@"
                        <html>
                        <body>
                            <h2>One-Time Password (OTP)</h2>
                            <p>Please use the following OTP:</p>
                            <h3 style=""color: #4CAF50;"">{otp}</h3>
                            <p>This OTP will expire in 15 minutes.</p>
                            <p>If you did not request this OTP, please ignore this email.</p>
                            <p>Thank you,<br>The POS System Team</p>
                        </body>
                        </html>
                    ";
                    break;
            }

            await SendEmailAsync(email, subject, body, true);
        }
    }
}
