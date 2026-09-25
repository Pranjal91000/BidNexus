namespace API.Models.Authentication
{
    public class LoginViewModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
