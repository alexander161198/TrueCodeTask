using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedModels;
using SharedModels.EntityModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserService.Managers
{
    public class TokenManager
    {
        private readonly JwtOptions _jwtOpt;

        public TokenManager(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOpt = jwtOptions.Value;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOpt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOpt.Issuer,
                audience: _jwtOpt.Issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtOpt.ExpireHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
