using CurrencyConverterAPI.Helper.Localization;
using CurrencyConverterAPI.Helpers;
using CurrencyConverterAPI.Services.Account;
using CurrencyConverterAPI.Services.Converter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

var configServices = builder.Configuration;

ConfigurationProviderCustom.ConfigurationProviders = configServices;


builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromSeconds(10);
        limiterOptions.QueueLimit = 0;
    });
});

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(5, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
        TimeSpan.FromMilliseconds(Random.Shared.Next(100, 500))
    );

builder.Services.AddHttpClient<IConverterService, ConverterService>()
    .AddPolicyHandler(retryPolicy);
builder.Services.AddHttpClient<IAccountService, AccountService>()
    .AddPolicyHandler(retryPolicy);

builder.Services.AddLocalization();
builder.Services.AddSingleton<LocalizationMiddleware>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddHttpContextAccessor();


// configure jwt authentication
RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
var rsaKeyPath = string.Concat(configServices.GetValue<string>("JWT:RSAKeyPath"), "/public_key.xml");
string publicPrivateKey = File.ReadAllText(rsaKeyPath);
provider.FromXmlString(publicPrivateKey);
RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(provider);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.NoResult();
            context.Response.StatusCode = 401;
            context.Response.WriteAsync("Unauthorized").Wait();
            return Task.CompletedTask;
        }
    };
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = rsaSecurityKey,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateAudience = false,
        ValidIssuer = configServices.GetValue<string>("JWT:Issuer")
    };
});

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IConverterService, ConverterService>();



// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRateLimiter(); // Apply rate limiting globally

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtCookieToHeaderMiddleware>();
app.UseMiddleware<LocalizationMiddleware>();

app.MapControllers();

app.Run();
