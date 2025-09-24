using Flow_Api.Middleware;

namespace Flow_Api.Extensions
{
    public static class AppExtensions
    {
        public static IApplicationBuilder UsePosMiddleware(this IApplicationBuilder app)
        {
            // Use middleware in the correct order
            app.UseMiddleware<SecurityHeadersMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<RateLimiterMiddleware>();

            // Enable CORS
            app.UseCors("AllowAll");

            // Enable authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
