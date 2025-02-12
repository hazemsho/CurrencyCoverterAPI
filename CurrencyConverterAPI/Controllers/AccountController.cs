using CurrencyConverterAPI.CustomHttpResponse;
using CurrencyConverterAPI.Enums;
using CurrencyConverterAPI.Models.Account;
using CurrencyConverterAPI.Services.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CurrencyConverterAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IAccountService _iAccountService;

        public AccountController(IAccountService iAccountService)
        {
            this._iAccountService = iAccountService;
        }

        [HttpPost]
        [AllowAnonymous]
        [EnableRateLimiting("fixed")]
        public async Task<IResponseType<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var response = await this._iAccountService.Login(request);
                return response;
            }
            catch (Exception ex)
            {
                return ResponseType<LoginResponseDto>.PerformError<LoginResponseDto>((int)ErrorCodeEnum.Exception, ex.Message, null, ex, HttpContext, true, true);
            }
        }
    }
}
