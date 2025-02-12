using CurrencyConverterAPI.Enums;
using Microsoft.Extensions.Localization;

namespace CurrencyConverterAPI.Helper
{
    public static class GeneralHelper
    {

        public static string CurrentLanguage(this HttpContext httpContext)
        {
            var language = Thread.CurrentThread.CurrentCulture.Name;

            return language == "ar-AE" ? "ar" : "en"; ;
        }

        public static bool IsRtl(this HttpContext httpContext)
        {
            var language = Thread.CurrentThread.CurrentCulture.Name;

            return language == "ar-AE";
        }

        public static string TranslateMessage(this IStringLocalizer<dynamic> localizer, int? errorCode, string errorMessage)
        {
            if (errorCode == null)
            {
                return null;
            }

            var errorCodeEnum = Enum.GetName(typeof(ErrorCodeEnum), (int)errorCode);
            if (errorCodeEnum == null)
            {
                return null;
            }

            var translatedMessage = localizer[errorCodeEnum]?.Value;
            if (string.IsNullOrEmpty(translatedMessage))
            {
                return errorMessage;
            }

            return translatedMessage;
        }

    }
}
