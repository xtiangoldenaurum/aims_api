using aims_api.Enums;
using aims_api.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace aims_api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class TokenController : ControllerBase
    {
        private readonly IOptions<AppSettings> _appSettings;

        public TokenController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        [HttpPost("gentoken")]
        public ActionResult GenToken(TokenAuthModel data)
        {
            try
            {
                if (data != null)
                {
                    if (data.TokenAuthUser != null & data.TokenAuthPass != null)
                    {
                        if (data.TokenAuthUser == _appSettings.Value.TokenUsername &&
                            data.TokenAuthPass == _appSettings.Value.TokenPassword)
                        {
                            // start creation of token
                            List<Claim> claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, data.TokenAuthUser ?? "")
                            };

                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.Key ?? ""));
                            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                            var token = new JwtSecurityToken(
                                                claims: claims,
                                                expires: DateTime.Now.AddDays(_appSettings.Value.TokenExpiryByDay),
                                                signingCredentials: cred
                                            );

                            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                            var tokenRes = new TokenResponseModel() { AuthToken = jwt, TokenExpiry = token.ValidTo};

                            return Ok(new RequestResponse(ResponseCode.SUCCESS, "Token generated successfuly.", tokenRes));
                        }
                    }
                }

                return StatusCode(401, new RequestResponse(ResponseCode.FAILED, "Unauthorized access."));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }
    }
}
