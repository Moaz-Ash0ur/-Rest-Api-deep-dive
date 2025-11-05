using System.Diagnostics;

namespace SampleWebApi.CustomMiddleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");

            context.Response.Headers.Add("x-Api-Version","v1");

            await _next(context); 

            stopwatch.Stop();
            Console.WriteLine($"Response:  [{context.Response.StatusCode}] ({stopwatch.ElapsedMilliseconds} ms)");
        }
    }

       public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }



}
