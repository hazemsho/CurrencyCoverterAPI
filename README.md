Authenticate:

Please call API http://localhost:5112/api/Account/Login as POST with parameters:
{
    "username":"guest",
    "password":"084e0343a0486ff05530df6c705c8bb4"
}

and the response will be as below:

{
    "data": {
        "token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Imd1ZXN0IiwibmFtZWlkIjoiZ3Vlc3QiLCJuYmYiOjE3MzkzNjIxMTEsImV4cCI6MTc0MDIyNjExMSwiaWF0IjoxNzM5MzYyMTExLCJpc3MiOiJsb2NhbGhvc3QifQ.hzF9wSSTa4MI6C9SkTISR8RSdTTj6IQLpjBq8PXofkcLMMT11Jmihj93kuFcvEFV5aKzNgN4vhrqptJqKnhUby_neaF2MFY5yyVOfGZQKwrvRtK3v9l0OD5tZanyRYrsGdy5BtuML14pYEy2V_Zf-1GxxmPdzg_GCy9rPpqMMGqo3vx1poeyXqUvSvpFMGnEt9V0UUYtLdgSJIQksjXcXh4zJ5qHzaQRpg2kAFFJpyFEQvtPws59Y5nkdheFHOHPOuaV7U-RSP8heV-ZyeCgQWX_x3g1aS9Mb3Lo9dhKX4A-GqE3-DPgqu1vvp13lmFwhFZAcIX-h5jk-c-YpXeUnw",
        "name": null
    },
    "message": null,
    "successed": true,
    "errorMessage": null,
    "errorCode": null,
    "modelState": null,
    "messageAr": null
}

so the token can be used in the other APIs to access

All APIs are GET

Get Rates API
http://localhost:5112/api/Converter/GetLatestRates?baseCurrency=USD

Convert API
http://localhost:5112/api/Converter/ConvertAmount?fromCurrency=USD&toCurrency=AUD&amount=1


History API
http://localhost:5112/api/Converter/GetHistoricalRates?baseCurrency=EUR&fromDate=2000-01-01&toDate=2000-12-31





-----------

For future enhancement, there are a lot of things can be added from storing the history data locally as it'll not be changed, 
and then implementing caching for getting the data based on the inputs .
