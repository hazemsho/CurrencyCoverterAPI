using CurrencyConverterAPI.CustomHttpResponse;

namespace CurrencyConverterAPI.Services.Converter
{
    public interface IConverterService
    {
        Task<IResponseType<string>> GetLatestRates(string baseCurrency);
        Task<IResponseType<decimal>> ConvertAmount(string fromCurrency, string toCurrency,decimal amount);
        Task<IResponseType<string>> GetHistoricalRates(string baseCurrency, string fromDate, string toDate);
    }
}
