namespace API.Models.Authentication
{
    public class LoginInputModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenInputModel
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}

