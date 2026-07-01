using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PlayCourt.Application.DTOs.Auth;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class JwtTokenService : IJwtTokenService
    {
        private const int DefaultAccessTokenExpiryMinutes = 60;
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtTokenResult GenerateAccessToken(User user, UserProfile? profile)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
            var issuer = jwtSection["Issuer"] ?? "PlayCourt";
            var audience = jwtSection["Audience"] ?? "PlayCourtClient";
            var expiresInMinutes = int.TryParse(jwtSection["ExpiresInMinutes"], out var minutes)
                ? minutes
                : DefaultAccessTokenExpiryMinutes;

            var expiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes);
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString()),
                new("role", user.Role.ToString())
            };

            if (!string.IsNullOrWhiteSpace(profile?.FullName))
            {
                claims.Add(new Claim(ClaimTypes.Name, profile.FullName));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtTokenResult
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt
            };
        }
    }
}
