using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using UserApi.Models;
using UserApi.Repository;

namespace UserApi.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        static readonly ILogger Log = Serilog.Log.ForContext<ExceptionMiddleware>();
        
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ExceptionModel errorModel)
            {
                await HandleExceptionAsync(httpContext, errorModel);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext httpContext, ExceptionModel ex)
        {

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = ex.StatusCode;
            var now = DateTime.UtcNow;
            Log.Error($"{now.ToString("HH:mm:ss")} : {ex}");
            return httpContext.Response.WriteAsync(ex.ToString());

        }
        
        private static Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return httpContext.Response.WriteAsync(ex.ToString());
        }
    }
}