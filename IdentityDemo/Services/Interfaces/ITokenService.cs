using Microsoft.AspNetCore.Identity;

namespace IdentityDemo.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(string username, string email, string id, string role);
    }
}