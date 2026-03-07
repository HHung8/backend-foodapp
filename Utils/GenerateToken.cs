using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class TokenService(IConfiguration config)
{
        public string GenerateToken(User user)
        {
                var secret = config["Jwt:Secret"]!;
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                        new Claim("id", user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                };

                var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(120),
                        signingCredentials: creds
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
        }
}