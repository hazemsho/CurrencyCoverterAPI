namespace CurrencyConverterAPI.Enums
{
    public enum ErrorCodeEnum
    {
        MissingUsername = 1,
        MissingPassword = 2,
        UserNotFound = 3,
        UserHasNoRoles = 4,
        InvalidUsernameOrPassword = 5,
        Exception = 6,
        ErrorResponse=7,
        MissingBaseCurrency = 8
    }
}
