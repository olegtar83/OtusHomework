using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CourierService.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    public LoggingBehaviour(ILogger<TRequest> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logging..");
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Courier Request: {Name}", requestName);

        await Task.CompletedTask;
    }
}
