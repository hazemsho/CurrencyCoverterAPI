using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System;

namespace CurrencyConverterAPI.Helpers.Localization
{
    public class TranslationProvider
    {
        public static string getTranslate(string code, string lang = "en")
        {
            try
            {
                string json_file;
                if (string.IsNullOrEmpty(lang))
                    lang = "en";

                lang = lang.ToLower() == "ar" ? "ar-AE" : "en-US";
                string ApiPhysicalPath = ConfigurationProviderCustom.GetConfiguration("ApiPhysicalPath").ToString();

                json_file = ApiPhysicalPath + "/Resources/" + lang.ToLower() + ".json";
                if (File.Exists(json_file))
                {
                    using (StreamReader r = new StreamReader(json_file))
                    {
                        string json = r.ReadToEnd();
                        var data = (JObject)JsonConvert.DeserializeObject(json);
                        if (data != null && data[code] != null)
                        {
                            string result = data[code].Value<string>();
                            if (result != null)
                                return result;
                        }
                        return code;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return code;
        }
    }
}
