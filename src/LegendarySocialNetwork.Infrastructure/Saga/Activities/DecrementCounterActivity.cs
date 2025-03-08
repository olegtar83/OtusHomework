using LegendarySocialNetwork.Domain.Messages;
using MassTransit;

namespace LegendarySocialNetwork.Infrastructure.Saga.Activities
{
    public class DecrementCounterActivity : IStateMachineActivity<CounterSagaStateInstance, MessageFailed>
    {
        private readonly ITopicProducer<string, DecrementCounter> _producer;

        public DecrementCounterActivity(ITopicProducer<string, DecrementCounter> producer)
        {
            _producer = producer;
        }
        public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

        public async Task Execute(BehaviorContext<CounterSagaStateInstance, MessageFailed> context, IBehavior<CounterSagaStateInstance, MessageFailed> next)
        {
            LogContext.Info?.Log($"Sent to counter service for decremention with CorrelationId {context.CorrelationId}");
            await _producer.Produce(Guid.NewGuid().ToString(), new DecrementCounter
            {
                From = context.Message.From,
                To = context.Message.To
            }, context.CancellationToken);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<CounterSagaStateInstance, MessageFailed, TException> context, IBehavior<CounterSagaStateInstance, MessageFailed> next) where TException : Exception
        {
            await next.Faulted(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context) => context.CreateScope(nameof(DecrementCounterActivity));
    }
}
