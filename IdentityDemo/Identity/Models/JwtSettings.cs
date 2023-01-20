namespace IdentityDemo.Identity.Models
{
    public class JwtSettings
    {
        public string SecretKey { get; init; }
        public int Expiration { get; init; }
        public bool RequiresHttps { get; init; }
    }
}
