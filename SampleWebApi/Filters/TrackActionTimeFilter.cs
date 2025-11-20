using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace SampleWebApi.Filters
{

    //Action Filter after and before endPoint
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TrackActionTimeFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            Console.WriteLine("Track Action Time Filter Started");


            context.HttpContext.Items["TimeExcute"] = DateTime.UtcNow;

            await next();

            var startTime = (DateTime)context.HttpContext.Items["TimeExcute"]!;

            var elapsed = DateTime.UtcNow - startTime;


            context.HttpContext.Response.Headers.Append("X-Elapsed-Time", $"{elapsed.TotalMilliseconds}ms");


            Console.WriteLine($"Track Action Time Filter Took {elapsed.TotalMilliseconds}ms");

        }


    }


    //Resource Filter after and before endPoint
    public class TanentValidationFilter(IConfiguration config) : IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //the request header should be have these data to check
            var tenantId = context.HttpContext.Request.Headers["tenantId"].ToString();
            var apiKey = context.HttpContext.Request.Headers["x-api-key"].ToString();


            var expectedKey = config[$"Tenants:{tenantId}:ApiKey"];


            if (string.IsNullOrEmpty(expectedKey) || expectedKey != apiKey)
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            await next();

        }
    }


    //Result filter after endPoint and before send response
    public class EnvelopeResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {

            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var apiResponse = new
                {
                    success = true,
                    data = objectResult.Value
                };


                context.Result = new JsonResult(apiResponse)
                {
                    StatusCode = objectResult.StatusCode
                };

            }

            await next();
        }




    }


}
