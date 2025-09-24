using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Flow_Api.Middleware
{
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RateLimiterMiddleware> _logger;
        private readonly int _maxRequests = 5; // Maximum requests
        private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(5); // Time window

        public RateLimiterMiddleware(
            RequestDelegate next,
            IMemoryCache cache,
            ILogger<RateLimiterMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Only apply rate limiting to authentication endpoints
            if (context.Request.Path.StartsWithSegments("/api/auth"))
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrEmpty(clientIp))
                {
                    await _next(context);
                    return;
                }

                var cacheKey = $"RateLimit_{clientIp}";

                // Get current request count
                var currentRequestCount = _cache.Get<int>(cacheKey);

                if (currentRequestCount >= _maxRequests)
                {
                    _logger.LogWarning($"Rate limit exceeded for IP: {clientIp}");
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                    return;
                }

                // Increment request count
                _cache.Set(cacheKey, currentRequestCount + 1, _timeWindow);
            }

            await _next(context);
        }
    }
}
