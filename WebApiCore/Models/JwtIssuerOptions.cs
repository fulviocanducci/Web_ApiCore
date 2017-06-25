using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace WebApiCore.Models
{
    public class JwtIssuerOptions
    {
        public string Issuer { get; set; }
        public string Subject { get; set; }        
        public string Audience { get; set; }
        public DateTime NotBefore => DateTimeOffset.Now.DateTime;
        public DateTime IssuedAt => DateTimeOffset.Now.DateTime;
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(2);
        public DateTime Expiration => DateTimeOffset.Now.DateTime.Add(ValidFor);
        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());
        public SigningCredentials SigningCredentials { get; set; }
    }    
}
