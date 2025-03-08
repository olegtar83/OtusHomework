using LegendarySocialNetwork.Counter.Services;
using LegendarySocialNetwork.Domain.Messages;
using MassTransit;

namespace LegendarySocialNetwork.Counter.Consumers
{
    public class CounterConsumer(ITarantoolService tarantoolService, ILogger<CounterConsumer> logger) : IConsumer<IncrementCounter>, IConsumer<DecrementCounter>
    {
        private ILogger<CounterConsumer> _logger { get; } = logger;

        public async Task Consume(ConsumeContext<IncrementCounter> context)
        {
            _logger.LogInformation($"{nameof(CounterConsumer)} activated for incremenation.");

            await tarantoolService.CountMessageIncrement(context.Message.From, context.Message.To);
        }

        public async Task Consume(ConsumeContext<DecrementCounter> context)
        {
            _logger.LogInformation($"{nameof(CounterConsumer)} activated for decremenation.");

            await tarantoolService.CountMessageDecrement(context.Message.From, context.Message.To);
        }
    }
}
