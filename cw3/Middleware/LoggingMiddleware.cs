using cw3.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace cw3.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILoggingService loggingService)
        {
            if (context.Request != null)
            {
                context.Request.EnableBuffering();

                string path = context.Request.Path;
                string method = context.Request.Method;
                string queryStr = context.Request.QueryString.ToString();
                string bodyStr = "";

                using(var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                loggingService.Log($"NEW REQUEST\npath = '{path}'\nmethod = '{method}'\nqueryStr = '{queryStr}'\nbodyStr = '\n{bodyStr}\n'\nEND OF REQUEST\n");

            }
            await _next(context);
        }
    }
}
