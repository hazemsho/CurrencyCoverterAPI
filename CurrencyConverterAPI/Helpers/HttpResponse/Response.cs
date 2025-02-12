namespace CurrencyConverterAPI.CustomHttpResponse
{
    public class Response
    {
        public bool Successed { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
