using Microsoft.Extensions.Configuration;
using System;

namespace CurrencyConverterAPI.Helpers
{
    public static class ConfigurationProviderCustom
    {
        public static IConfiguration ConfigurationProviders { get; set; }

        public static string GetConfiguration(string key)
        {
            string configurationValue = null;

            var item = ConfigurationProviders[key];

            if (item != null)
                configurationValue = item;
            else
            {
                throw new Exception(string.Format("Configuration Key not found: {0}", key));
            }

            return configurationValue;
        }
    }
}
