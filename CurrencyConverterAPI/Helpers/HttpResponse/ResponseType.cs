using System.Data;
using System.Data.SqlClient;
using System;
using System.Threading;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using CurrencyConverterAPI.Enums;
using CurrencyConverterAPI.Helpers.Localization;

namespace CurrencyConverterAPI.CustomHttpResponse
{

    public class ResponseType<TType> : IResponseType<TType>
    {
        public TType Data { get; set; }

        public string Message { get; set; }

        public bool Successed { get; set; }

        public string ErrorMessage { get; set; }

        public int? ErrorCode { get; set; }

        public dynamic ModelState { get; set; }

        public string MessageAr { get; set; }

        public static ResponseType<T> PerformSuccessed<T>(dynamic data)
        {
            return new ResponseType<T>
            {
                Successed = true,
                Data = data,
            };
        }
        public static ResponseType<T> PerformSuccessed<T>(dynamic data, string successMessage, int? errorCode = null)
        {
            return new ResponseType<T>
            {
                Successed = true,
                Data = data,
                Message = successMessage,
                ErrorCode = errorCode
            };
        }
        public static ResponseType<T> PerformError<T>(int errorCode, string errorMessage = null, dynamic data = null, Exception ex = null, HttpContext httpContext = null, bool saveLog = false, bool translateMessage=true)
        {
            if (saveLog || ex != null)
                LogExceptionAsync(ex, errorMessage, httpContext);

            if(translateMessage)
            {
                var errorCodeString = Enum.GetName(typeof(ErrorCodeEnum), errorCode);
                var translatedMessage = TranslationProvider.getTranslate(errorCodeString, Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
                if (!string.IsNullOrEmpty(translatedMessage))
                {
                    errorMessage = translatedMessage;
                }
            }
            if (data != null)
            {
                return new ResponseType<T>
                {
                    Successed = false,
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage,
                    Data = data
                };
            }
            else
            {
                return new ResponseType<T>
                {
                    Successed = false,
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage
                };
            }
        }

        public static async Task LogExceptionAsync(
            Exception ex, 
            string addInfo = null, 
            HttpContext httpContext = null)
        {

            try
            {

                string AditionalInfo = null;
                if (ex != null)
                {
                    AditionalInfo = AditionalInfo == null ? "" : AditionalInfo;
                    try
                    {
                        var innerException = (ex.InnerException);
                        if (innerException != null)
                        {
                            AditionalInfo += innerException.Message;
                        }

                    }
                    catch (Exception ee)
                    {
                        try
                        {
                            AditionalInfo += ex.Message;
                        }
                        catch (Exception eee)
                        {
                            //AditionalInfo += eee.ToString();
                        }
                    }

                    AditionalInfo = AditionalInfo == null ? ex.ToString() : string.Concat(AditionalInfo, " - ", ex.ToString());
                    if (!string.IsNullOrEmpty(addInfo))
                    {
                        if (string.IsNullOrEmpty(AditionalInfo))
                            AditionalInfo = addInfo;
                        else if (!string.IsNullOrEmpty(AditionalInfo))
                            AditionalInfo = string.Concat(AditionalInfo, " - ", addInfo);
                    }

                }
                else
                {
                    AditionalInfo = addInfo;
                }

                // we can store the log into the database
                // but it needs time to build the db, or we can store it in log file

                //var ErrorCodeP = new SqlParameter("@ErrorCode", SqlDbType.Int);
                //ErrorCodeP.Value = (int?)null;
                //ErrorCodeP.IsNullable = true;

                //var ErrorMsgP = new SqlParameter("@ErrorMsg", SqlDbType.NVarChar);
                //ErrorMsgP.Value = ex?.Message;

                //var AditionalInfoP = new SqlParameter("@AditionalInfo", SqlDbType.NVarChar);
                //AditionalInfoP.Value = AditionalInfo;

                //var UserIdP = new SqlParameter("@UserId", SqlDbType.Int);
                //UserIdP.Value = httpContext != null ? httpContext.CurrentLoginUserId() : (int?)null;
                //UserIdP.IsNullable = true;


                //List<SqlParameter> list1 = new List<SqlParameter>()
                //{
                //    ErrorCodeP,
                //    ErrorMsgP,
                //    AditionalInfoP,
                //    UserIdP
                //};

                //HelperContext.ExecuteCustomSP("AddSystemLogs", list1);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
