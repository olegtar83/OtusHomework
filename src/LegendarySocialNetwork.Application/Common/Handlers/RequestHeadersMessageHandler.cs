using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LegendarySocialNetwork.Application.Common.Handlers
{
    public class RequestHeadersMessageHandler : DelegatingHandler
    {
        private readonly IServiceProvider _sp;

        public RequestHeadersMessageHandler(IServiceProvider serviceProvider)
        {
            _sp = serviceProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContextAccessor = _sp.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var requestIdHeader = httpContext?.Request.Headers["X-RequestId"].FirstOrDefault();
            if (requestIdHeader != null)
                request.Headers.Add("X-RequestId", requestIdHeader);
            
            request.Headers.Add("X-Api-Version", Environment.GetEnvironmentVariable("Messages:ApiVersion"));

            var authHeader = httpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null)
                request.Headers.Add("Authorization", authHeader);

            return base.SendAsync(request, cancellationToken);
        }

    }
}
