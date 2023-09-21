using Microsoft.AspNetCore.Builder;

namespace UserApi.Middleware.OptionsMiddleware
{
    public static class OptionsMiddlewareExtensions
    {
        public static IApplicationBuilder UseOptions(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OptionsMiddleware>();
        }
    }
}
