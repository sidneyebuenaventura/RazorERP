using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RazorERP.Middleware
{
    public class ThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, DateTime> _requestTimes = new ConcurrentDictionary<string, DateTime>();
        private const int REQUEST_LIMIT = 10;
        private const int TIME_WINDOW = 60; // in seconds

        public ThrottlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var key = $"{context.Request.Path}-{context.Connection.RemoteIpAddress}";

            if (_requestTimes.TryGetValue(key, out var lastRequestTime))
            {
                if (lastRequestTime.AddSeconds(TIME_WINDOW) > DateTime.UtcNow && _requestTimes.Count >= REQUEST_LIMIT)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Too many requests. Please wait a moment and try again.");
                    return;
                }
            }

            _requestTimes[key] = DateTime.UtcNow;

            await _next(context);
        }
    }
}
