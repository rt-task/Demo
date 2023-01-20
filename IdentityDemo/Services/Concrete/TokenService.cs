using IdentityDemo.Identity.Models;
using IdentityDemo.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityDemo.Services.Concrete
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string CreateToken(string username, string email, string id, string role)
        {
            var claims = CreateClaims(username, email, id, role);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(null, null, claims, DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(_jwtSettings.Expiration), signingCredentials);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return accessToken;
        }

        private List<Claim> CreateClaims(string username, string email, string id, string role)
        {
            return new()
            {
                new (ClaimTypes.NameIdentifier, id),
                new (ClaimTypes.Name, username),
                new (ClaimTypes.Email, email),
                new (ClaimTypes.Role, role)
            };
        }
    }
}
