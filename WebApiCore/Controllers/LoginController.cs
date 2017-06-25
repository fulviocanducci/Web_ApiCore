using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiCore.Models;
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System;
namespace WebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/login")]
    public class LoginController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        public LoginController(ILoggerFactory loggerFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _logger = loggerFactory.CreateLogger<LoginController>();
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ObjectResult> Post(User user)
        {
            ClaimsIdentity identity = await GetClaimsIdentity(user);

            if (identity == null)
            {
                _logger.LogInformation($"Invalid email ({user.Email}) or password ({user.Password})");
                return BadRequest("Invalid credentials");
            }

            Claim[] claims = new Claim[4]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, _jwtOptions.IssuedAt.ToUnixEpochDateToString(), ClaimValueTypes.Integer64),
                identity.FindFirst("Auth")
            };

            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler()
                .WriteToken(jwt);
            
            var response = new
            {
                access_token = encodedJwt,
                expires_seconds = (int)_jwtOptions.ValidFor.TotalSeconds, 
                expires_minutes = _jwtOptions.ValidFor.Minutes,
                expires_create = _jwtOptions.NotBefore.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
                expires_remove = _jwtOptions.Expiration.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
                expires_remove0 = DateTimeOffset.Now.ToString(),
                time_zone = TimeZoneInfo.Local.StandardName
            };

            return Ok(response);
        }

        private Task<ClaimsIdentity> GetClaimsIdentity(User user)
        {           
            if (user.Email == "fulviocanducci@hotmail.com" && user.Password == "123456")
            {
                return Task.FromResult(new ClaimsIdentity
                    (
                        new GenericIdentity(user.Email, "Token"),
                        new[] {
                            new Claim("Auth", "WebApi")
                        }
                    )
                 );
            }            
            return Task.FromResult<ClaimsIdentity>(null);
        }
        
    }
}