using Microsoft.IdentityModel.Tokens;
namespace WebApiCore.Models
{
    public class JwtSettings
    {
        public SymmetricSecurityKey SecurityKey { get; set; }
        public int TokenExpiration { get; set; }
        public string Audience { get; set; } = "DummyAudience";
        public string Issuer { get; set; } = "DummyIssuer";

        public JwtSettings(SymmetricSecurityKey securityKey, int tokenExpiration)
        {
            SecurityKey = securityKey;
            TokenExpiration = tokenExpiration;
        }
    }
}
