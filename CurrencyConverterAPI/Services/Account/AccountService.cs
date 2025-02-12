using CurrencyConverterAPI.CustomHttpResponse;
using CurrencyConverterAPI.Enums;
using CurrencyConverterAPI.Helper;
using CurrencyConverterAPI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CurrencyConverterAPI.Services.Account
{
    public class AccountService : IAccountService
    {
        private IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
           IConfiguration configuration,
           IHttpContextAccessor httpContextAccessor)
        {
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<IResponseType<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var lang = this._httpContextAccessor.HttpContext.CurrentLanguage();
            var result = new LoginResponseDto();
            if (string.IsNullOrEmpty(request.Username))
            {
                return ResponseType<LoginResponseDto>.PerformError<LoginResponseDto>((int)ErrorCodeEnum.MissingUsername, "Missing username", null, null, this._httpContextAccessor.HttpContext, false, true);
            }
            if (string.IsNullOrEmpty(request.Password))
            {
                return ResponseType<LoginResponseDto>.PerformError<LoginResponseDto>((int)ErrorCodeEnum.MissingPassword, "Missing password", null, null, this._httpContextAccessor.HttpContext, false, true);
            }

            if (!request.Username.Equals("guest"))
            {
                return ResponseType<LoginResponseDto>.PerformError<LoginResponseDto>((int)ErrorCodeEnum.UserNotFound, "User not found", null, null, this._httpContextAccessor.HttpContext, false, true);
            }


            if (!request.Username.Equals("quest") && !request.Password.Equals("084e0343a0486ff05530df6c705c8bb4"))
            {
                return ResponseType<LoginResponseDto>.PerformError<LoginResponseDto>((int)ErrorCodeEnum.InvalidUsernameOrPassword, "Invalid username or password", null, null, this._httpContextAccessor.HttpContext, false, true);
            }
            var tokenExpiry = DateTime.Now.AddDays(Convert.ToInt32(this._configuration["JWT:TokenExpiry:Days"])).AddMinutes(Convert.ToInt32(this._configuration["JWT:TokenExpiry:Minutes"]));


            RsaSecurityKey rsaSecurityKey = getRSAXmlPrivateKey(this._configuration["JWT:RSAKeyPath"]);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, request.Username), new Claim(ClaimTypes.NameIdentifier, request.Username) }),
                Expires = tokenExpiry,
                SigningCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256),
                Issuer = _configuration["JWT:Issuer"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            result.Token = tokenHandler.WriteToken(token);

            this._httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", result.Token, new CookieOptions() { Expires = tokenExpiry });

            return ResponseType<LoginResponseDto>.PerformSuccessed<LoginResponseDto>(result);



        }
        public static RsaSecurityKey getRSAXmlPrivateKey(string rsaKeyPath)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            string publicPrivateKey = File.ReadAllText(string.Concat(rsaKeyPath, "/", "private_key.xml"));
            provider.FromXmlString(publicPrivateKey);
            RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(provider);
            return rsaSecurityKey;
        }

    }


}
