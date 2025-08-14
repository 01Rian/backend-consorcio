using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.Src.Factories
{
    public static class JwtTokenFactory
    {
        public static string Create(Usuario user)
        {
            var claims = new List<Claim>
            {
                new Claim("uid", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("name", user.Nome)
            };

            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new InvalidOperationException("JWT_SECRET não configurado");
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new InvalidOperationException("JWT_ISSUER não configurado");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT_AUDIENCE não configurado");
            var expiryMinutesStr = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") ?? "60";
            if (!int.TryParse(expiryMinutesStr, out int expiryMinutes)) expiryMinutes = 60;

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }
    }
}
