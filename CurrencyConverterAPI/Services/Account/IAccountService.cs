using CurrencyConverterAPI.CustomHttpResponse;
using CurrencyConverterAPI.Models.Account;

namespace CurrencyConverterAPI.Services.Account
{
    public interface IAccountService
    {
        Task<IResponseType<LoginResponseDto>> Login(LoginRequestDto request);
    }
}
