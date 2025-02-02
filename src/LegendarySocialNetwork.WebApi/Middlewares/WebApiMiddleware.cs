
namespace LegendarySocialNetwork.WebApi.Middlewares
{
    public class WebApiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        public WebApiMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var guid = Guid.NewGuid().ToString();

            context.Request.Headers.Append("X-RequestId", guid);

            _logger.LogInformation($"X-RequestId {guid} header added to http request");

            await _next(context);
        }
    }
}
