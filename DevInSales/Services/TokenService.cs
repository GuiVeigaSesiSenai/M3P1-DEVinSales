using DevInSales.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DevInSales.Services
{
    public static class TokenService
    {
        public static string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("T7kCvXPeppDMWuPjPmQHY5mKaUNuaMV4Y78YWE3s38HASErwbMjdby7JX5qRfJsV");

            var tokenTokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new Claim[]
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name), // User.Identity.Name
                    new Claim(ClaimTypes.Role, user.Profile.Name) // User.IsInRole
                }),

                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenTokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
