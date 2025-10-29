namespace Flow_Api.Dtos.Auth.Response
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserInfoResponseDto User { get; set; } = null!;
    }
}
