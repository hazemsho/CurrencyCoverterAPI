namespace CurrencyConverterAPI.CustomHttpResponse
{
    public interface IResponseType<TType>
    {
        TType Data { get; set; }

        string Message { get; set; }

        bool Successed { get; set; }

        string ErrorMessage { get; set; }

        int? ErrorCode { get; set; }

    }
}
