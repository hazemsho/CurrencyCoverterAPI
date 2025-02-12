using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyConverterAPI.Helper.Localization
{
    public class LocalizationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string languageParameter = context.Request.Query["language"];
            string langParameter = context.Request.Query["lang"];
            var language = context.Request.Headers["Language"];
            var cultureKey = context.Request.Cookies["Language"];
            if (string.IsNullOrEmpty(cultureKey))
            {
                var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();

                // Get the first language from the header
                var languages = acceptLanguage?.Split(',').Select(lang => lang.Split(';').First()).ToList();
                var browserLanguage = languages?.FirstOrDefault();

                if (browserLanguage.StartsWith("ar"))
                {
                    cultureKey = "ar-AE";
                }
                else
                {
                    cultureKey = "en-US";
                }
            }

            if (!string.IsNullOrEmpty(language))
            {
                if (language == "ar" || language == "ar-AE")
                {
                    cultureKey = "ar-AE";
                }
                else if (language == "en" || language == "en-US")
                {
                    cultureKey = "en-US";
                }
            }

            if (!string.IsNullOrEmpty(languageParameter))
            {
                if (languageParameter == "ar" || languageParameter == "ar-AE")
                {
                    cultureKey = "ar-AE";
                }
                else if (languageParameter == "en" || languageParameter == "en-US")
                {
                    cultureKey = "en-US";
                }
            }
            else if (!string.IsNullOrEmpty(langParameter))
            {
                if (langParameter == "ar" || langParameter == "ar-AE")
                {
                    cultureKey = "ar-AE";
                }
                else if (langParameter == "en" || langParameter == "en-us")
                {
                    cultureKey = "en-US";
                }
            }
            var culture = new CultureInfo(cultureKey);
            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.NumberFormat.NumberGroupSeparator = ",";
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            if (!context.Response.HasStarted)
            {
                context.Response.Cookies.Append("Language", cultureKey, new CookieOptions() { Expires = DateTime.UtcNow.AddYears(5) });
            }
            await next(context);
        }
    }
}
