namespace Flow_Api.Services.Interfaces.Notifications
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string ownerName, string shopName);
        Task SendApprovalEmailAsync(string toEmail, string ownerName, string shopName, string loginUrl);
        Task SendRejectionEmailAsync(string toEmail, string ownerName, string reason);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}
