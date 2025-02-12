using CurrencyConverterAPI.CustomHttpResponse;
using CurrencyConverterAPI.Enums;
using CurrencyConverterAPI.Services.Account;
using CurrencyConverterAPI.Services.Converter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CurrencyConverterAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ConverterController : Controller
    {
        private readonly IConverterService _iConverterService;

        public ConverterController(IConverterService iConverterService)
        {
            this._iConverterService = iConverterService;
        }

        [HttpGet]
        [Authorize]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetLatestRates(string baseCurrency)
        {
            try
            {
                var response = await this._iConverterService.GetLatestRates(baseCurrency);
                if (response.Successed)
                {
                    return Ok(response.Data);
                }
                return BadRequest(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                await ResponseType<object>.LogExceptionAsync(ex, ex.ToString(), this.HttpContext);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> ConvertAmount(string fromCurrency, string toCurrency, decimal amount)
        {
            try
            {
                if (string.IsNullOrEmpty(toCurrency))
                {
                    var notAllowedCurrencies = new List<string>(["TRY", "PLN", "THB", "MXN"]);
                    if (notAllowedCurrencies.Any(a => a == toCurrency))
                    {
                        return BadRequest("Not allwoed currecny");
                    }
                }
                var response = await this._iConverterService.ConvertAmount(fromCurrency, toCurrency, amount);

                if (response.Successed)
                {
                    return Ok(response.Data);
                }
                return BadRequest(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                await ResponseType<object>.LogExceptionAsync(ex, ex.ToString(), this.HttpContext);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetHistoricalRates(string baseCurrency, string fromDate, string toDate)
        {
            try
            {
                var response = await this._iConverterService.GetHistoricalRates(baseCurrency, fromDate, toDate);

                if (response.Successed)
                {
                    return Ok(response.Data);
                }
                return BadRequest(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                await ResponseType<object>.LogExceptionAsync(ex, ex.ToString(), this.HttpContext);
                return BadRequest(ex.Message);
            }
        }
    }
}
