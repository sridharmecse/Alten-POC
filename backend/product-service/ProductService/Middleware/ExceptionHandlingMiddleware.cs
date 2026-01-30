using System.Net;
using System.Text.Json;

namespace ProductService.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next; _logger = logger;
        }
        public async Task Invoke(HttpContext ctx)
        {
            try { await _next(ctx); }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Bad request: {Message}", ex.Message);
                ctx.Response.ContentType = "application/json";
                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { title = ex.Message }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                ctx.Response.ContentType = "application/json";
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { title = "Unexpected error" }));
            }
        }
    }
}
