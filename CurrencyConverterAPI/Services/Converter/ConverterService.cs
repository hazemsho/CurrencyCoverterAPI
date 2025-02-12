using CurrencyConverterAPI.CustomHttpResponse;
using CurrencyConverterAPI.Enums;
using Newtonsoft.Json;
using System;
using System.Data;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CurrencyConverterAPI.Models.Converter;
using CurrencyConverterAPI.Models.Account;

namespace CurrencyConverterAPI.Services.Converter
{
    public class ConverterService : IConverterService
    {
        private IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConverterService(
           IConfiguration configuration,
           IHttpContextAccessor httpContextAccessor)
        {
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<IResponseType<string>> GetLatestRates(string baseCurrency)
        {
            if (string.IsNullOrEmpty(baseCurrency))
            {
                return ResponseType<string>.PerformError<string>((int)ErrorCodeEnum.MissingBaseCurrency, "Missing Base Currency", null, null, this._httpContextAccessor.HttpContext, false, true);
            }

            var url = string.Concat(this._configuration["FrankfurterAPIBaseUrl"], "/v1/latest?base=", baseCurrency);
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    using (var streamReader = new StreamReader(response.Content.ReadAsStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        return ResponseType<string>.PerformSuccessed<string>(result);
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    if (error != null)
                    {
                        error = string.Concat(response.ReasonPhrase, " - ", error);
                    }
                    return ResponseType<string>.PerformError<string>((int)ErrorCodeEnum.Exception, error, null, null, this._httpContextAccessor.HttpContext, true, false);
                }
            }
        }

        public async Task<IResponseType<decimal>> ConvertAmount(string fromCurrency, string toCurrency, decimal amount)
        {

            var url = string.Concat(this._configuration["FrankfurterAPIBaseUrl"], "/v1/latest?base=", fromCurrency, "&symbols=", toCurrency);
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    using (var streamReader = new StreamReader(response.Content.ReadAsStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        try
                        {
                            var exchangeData = JsonConvert.DeserializeObject<ExchangeRateDto>(result);

                            if (exchangeData == null)
                            {
                                return ResponseType<decimal>.PerformError<decimal>((int)ErrorCodeEnum.ErrorResponse, result, null, null, this._httpContextAccessor.HttpContext, true);
                            }
                            if (!exchangeData.Rates.ContainsKey(toCurrency))
                            {
                                return ResponseType<decimal>.PerformError<decimal>((int)ErrorCodeEnum.ErrorResponse, "To currency not available", null, null, this._httpContextAccessor.HttpContext, true);
                            }

                            var responseAmount = amount * exchangeData.Rates[toCurrency];

                            return ResponseType<decimal>.PerformSuccessed<decimal>(responseAmount);
                        }
                        catch (Exception ex)
                        {
                            return ResponseType<decimal>.PerformError<decimal>((int)ErrorCodeEnum.ErrorResponse, result, null, null, this._httpContextAccessor.HttpContext, true);
                        }

                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    if (error != null)
                    {
                        error = string.Concat(response.ReasonPhrase, " - ", error);
                    }
                    return ResponseType<decimal>.PerformError<decimal>((int)ErrorCodeEnum.Exception, error, null, null, this._httpContextAccessor.HttpContext, true, false);
                }
            }
        }

        public async Task<IResponseType<string>> GetHistoricalRates(string baseCurrency, string fromDate, string toDate)
        {
            // pagination can be implemented if the remote api already supported, but based on the documentation it's not
            // or can store the data locally and then do the pagination based on some inputs from the api like skip & take

            var url = string.Concat(this._configuration["FrankfurterAPIBaseUrl"], "/v1/", fromDate, "..", toDate, "?base=", baseCurrency);
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    using (var streamReader = new StreamReader(response.Content.ReadAsStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        return ResponseType<string>.PerformSuccessed<string>(result);
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    if (error != null)
                    {
                        error = string.Concat(response.ReasonPhrase, " - ", error);
                    }
                    return ResponseType<string>.PerformError<string>((int)ErrorCodeEnum.Exception, error, null, null, this._httpContextAccessor.HttpContext, true, false);
                }
            }
        }
    }
}
