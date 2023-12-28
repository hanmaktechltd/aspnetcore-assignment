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
       
        private readonly string issuer;
        private readonly IConfiguration configuration;
        private readonly string secretKey= "NVRBpevqncT8sw8gZMysnaRJ3Ww/ha6c9evS3TzcQBU=";



        public JwtAuthenticationService(IConfiguration configuration, string issuer)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(secretKey));
            this.issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));

            // Retrieve the secret key from app settings
           

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
                    // Add more claims if needed (e.g., roles, permissions)
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Token expiration time
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

                // Validate the token
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

                // If the token validation succeeds and the user is authenticated, return true
                return claimsPrincipal.Identity?.IsAuthenticated == true;
            }
            catch (Exception)
            {
                // Token validation failed or exception occurred
                return false;
            }
        }

        private string GenerateSecretKey()
        {
            byte[] bytes = new byte[32]; // Adjust the byte length as needed for your key
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            string secretKey = Convert.ToBase64String(bytes);

            return secretKey; // Return the generated secret key
        }
    }
}
