using cw3.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace cw3.Services
{
    public class JwtTokenGeneratorService : ITokenGeneratorService
    {
        public IConfiguration Configuration { get; private set; }

        public JwtTokenGeneratorService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string GetToken(LoginDetailsDTO loginDetails)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, loginDetails.IndexNumber),
                new Claim(ClaimTypes.Role, loginDetails.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
