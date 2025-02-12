using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverterAPI.Helpers
{
    public class JwtCookieToHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtCookieToHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var jwtFound = context.Request.Cookies.TryGetValue("jwt", out string jwtToken);

            if (!context.Request.Headers.ContainsKey("Authorization") && jwtFound)
            {
                if (!string.IsNullOrEmpty(jwtToken))
                    context.Request.Headers.Add("Authorization", $"Bearer {jwtToken}");
            }
            else
            {
                if (context.Request.Headers.ContainsKey("Authorization") && context.Request.Cookies.TryGetValue("jwt", out string jwtToken1))
                {
                    if (!string.IsNullOrEmpty(jwtToken1))
                    {
                        var authToken = context.Request.Headers["Authorization"].FirstOrDefault();
                        if (!string.IsNullOrEmpty(authToken))
                        {
                            authToken = authToken.Replace("Bearer ", "");
                            if (authToken != jwtToken1)
                            {

                                context.Request.Headers["Authorization"] = string.Concat("Bearer ", jwtToken1);
                            }
                        }
                    }

                }
            }


            await _next(context);

        }
    }
}
