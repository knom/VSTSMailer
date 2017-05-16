//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using System.Text.Encodings.Web;
//using Microsoft.Extensions.Options;

//namespace VSTSWebApi.Helpers
//{
//    public static class ApiKeyExtensions
//    {
//        public static IApplicationBuilder UseApiKey(this IApplicationBuilder builder)
//        {
//            return builder.UseMiddleware<ApiKeyMiddleware>();
//        }
//    }
    
//    public class ApiKeyMiddleware : AuthenticationMiddleware<ApiKeyAuthenticationOptions>
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger _logger;

//        public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyAuthenticationOptions> options, ILoggerFactory loggerFactory, UrlEncoder encoder):base(next, options, loggerFactory, encoder)
//        {
//            var i = 0;
//        }

//        //public async Task Invoke(HttpContext context)
//        //{
//        //    _logger.LogInformation("Handling API key for: " + context.Request.Path);

//        //    await _next.Invoke(context);

//        //    _logger.LogInformation("Finished handling api key.");
//        //}

//        protected override AuthenticationHandler<ApiKeyAuthenticationOptions> CreateHandler()
//        {
//            return new ApiKeyAuthenticationHandler();
//        }
//    }

//    internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
//    {
//        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
//        {
//            return Task.FromResult(AuthenticateResult.Success(null));
//        }
//    }

//    public class ApiKeyAuthenticationOptions : AuthenticationOptions
//    {
//        public ApiKeyAuthenticationOptions()
//        {            
//        }
//    }
//}