using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NS.Quizzy.Server.BL.Services
{
    internal class JwtHelper
    {
        private readonly byte[] _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly double _expiresInMinutes;
        private readonly double _expiresInMinutesForMobile;

        internal byte[] GetJwtKey() => [.. _key];
        internal string GetJwtIssuer() => _issuer;
        internal string GetJwtAudience() => _audience;
        internal double GetJwtExpiresInMinutes(bool isMobile) => isMobile ? _expiresInMinutesForMobile : _expiresInMinutes;

        public JwtHelper(IConfiguration configuration)
        {
            _issuer = configuration.GetValue<string>("AppIssuer") ?? "";

            var jwtSection = configuration.GetSection("Jwt") ?? throw new Exception("Jwt section not found");
            var keyStr = jwtSection.GetValue<string>("Key") ?? throw new Exception("Jwt key not found");
            _key = Encoding.UTF8.GetBytes(keyStr);
            _audience = jwtSection.GetValue<string>("Audience") ?? "";
            _expiresInMinutes = jwtSection.GetValue<double>("ExpiresInMinutes");
            _expiresInMinutesForMobile = jwtSection.GetValue<double>("ExpiresInMinutesForMobile");
        }

        public (Guid tokenId, string token) GenerateToken(Guid userId, string email, string fullName, DAL.DALEnums.Roles role)
        {
            var expires = DateTime.Now.AddMinutes(_expiresInMinutes);
            var tokenId = Guid.NewGuid();
            var tokenIdStr = tokenId.ToString();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.NameIdentifier,  userId.ToString()),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Sid, tokenIdStr),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, tokenIdStr)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return (tokenId, tokenHandler.WriteToken(token));
        }
    }
}
