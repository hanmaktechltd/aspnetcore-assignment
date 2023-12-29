using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Queue_Management_System.Models;

namespace Queue_Management_System.Services
{
    public class JwtAuthenticationService
    {
        private string secretKey = UserUtility.secretKey;
        private readonly string issuer;
        private readonly IConfiguration configuration;
       
        public JwtAuthenticationService(IConfiguration configuration, string issuer)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(secretKey));
            this.issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));

           

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Secret key is missing or invalid in the configuration.");
            }
        }

        public string GenerateToken(LoginViewModel admin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                     new Claim(ClaimTypes.Name, admin.UsernameOrEmail),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.Role, "ServiceProvider"),
                    new Claim(ClaimTypes.Authentication, "Auth"),
                }),
                Expires = DateTime.UtcNow.AddDays(7), 
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return claimsPrincipal.Identity?.IsAuthenticated == true;
            }
            catch (Exception)
            {
                return false;
            }
        }

       
    }
}
