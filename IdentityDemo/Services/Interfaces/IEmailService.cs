namespace IdentityDemo.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendResetPasswordTokenAsync(string username, string email, string token);
    }
}