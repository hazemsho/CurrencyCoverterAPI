namespace CurrencyConverterAPI.Models.Converter
{
    public class ExchangeRateDto
    {
        public double Amount { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
