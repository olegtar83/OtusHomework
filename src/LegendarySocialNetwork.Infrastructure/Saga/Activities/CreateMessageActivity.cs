using LegendarySocialNetwork.Domain.Messages;
using MassTransit;

namespace LegendarySocialNetwork.Infrastructure.Saga.Activities
{
    public class CreateMessageActivity : IStateMachineActivity<CounterSagaStateInstance, InitSaga>
    {
        private readonly ITopicProducer<string, CreateMessage> _producer;

        public CreateMessageActivity(ITopicProducer<string, CreateMessage> producer)
        {
            _producer = producer;
        }

        public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

        public async Task Execute(BehaviorContext<CounterSagaStateInstance, InitSaga> context, IBehavior<CounterSagaStateInstance, InitSaga> next)
        {
            LogContext.Info?.Log($"Sent to message service with CorrelationId {context.CorrelationId}");
            await _producer.Produce(Guid.NewGuid().ToString(), new CreateMessage
            {
                From = context.Message.From,
                To = context.Message.To,
                Text = context.Message.Text
            }, context.CancellationToken); ;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<CounterSagaStateInstance, InitSaga, TException> context, IBehavior<CounterSagaStateInstance, InitSaga> next) where TException : Exception
        {
            await next.Faulted(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context) => context.CreateScope(nameof(CreateMessageActivity));
    }
}
