namespace IdentityDemo.Models
{
    public class SmtpConfiguration
    {
        public string Host { get; init; }
        public int Port { get; init; }
        public bool UseSsl { get; init; }
        public string Username { get; init; }
        public string Password { get; init; }
        public object BaseUrl { get; init; }
    }
}