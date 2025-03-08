using LegendarySocialNetwork.Domain.Messages;
using MassTransit;

namespace LegendarySocialNetwork.Infrastructure.Saga.Activities
{
    public class IncrementCounterActivity : IStateMachineActivity<CounterSagaStateInstance, InitSaga>
    {
        private readonly ITopicProducer<string, IncrementCounter> _producer;

        public IncrementCounterActivity(ITopicProducer<string, IncrementCounter> producer)
        {
            _producer = producer;
        }

        public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

        public async Task Execute(BehaviorContext<CounterSagaStateInstance, InitSaga> context, IBehavior<CounterSagaStateInstance, InitSaga> next)
        {
            LogContext.Info?.Log($"Sent to counter service for incremention with CorrelationId {context.CorrelationId}");
            await _producer.Produce(Guid.NewGuid().ToString(), new IncrementCounter
            {
                From = context.Message.From,
                To = context.Message.To
            }, context.CancellationToken);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<CounterSagaStateInstance, InitSaga, TException> context, IBehavior<CounterSagaStateInstance, InitSaga> next) where TException : Exception
        {
            await next.Faulted(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context) => context.CreateScope(nameof(IncrementCounterActivity));

    }
}
