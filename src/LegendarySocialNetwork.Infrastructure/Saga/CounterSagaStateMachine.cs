using LegendarySocialNetwork.Domain.Messages;
using LegendarySocialNetwork.Infrastructure.Saga.Activities;
using MassTransit;

namespace LegendarySocialNetwork.Infrastructure.Saga
{
    public class CounterSagaStateMachine : MassTransitStateMachine<CounterSagaStateInstance>
    {
        public CounterSagaStateMachine()
        {
            InstanceState(x => x.CurrentState);

            CorrelateEvents();

            Initially(
                When(InitSaga)
                    .Then(context =>
                    {
                        LogContext.Info?.Log($"Counter Incrementing requested: {context.CorrelationId}");
                        context.Saga.CurrentDate = DateTime.UtcNow;
                        context.Saga.To = context.Message.To;
                        context.Saga.From = context.Message.From;
                    })
                    .Activity(config => config.OfType<IncrementCounterActivity>())
                    .Activity(config => config.OfType<CreateMessageActivity>())
                    .TransitionTo(WaitingForMessageCreation));

            During(WaitingForMessageCreation,
                When(MessageFailed)
                    .Then(context =>
                    {
                        LogContext.Info?.Log("Message Failed: {CorrelationId}", context.CorrelationId);
                        context.Saga.UpdatedAt = DateTime.UtcNow;
                        context.Saga.To = context.Message.To;
                        context.Saga.From = context.Message.From;
                    })
                    .Activity(config => config.OfType<DecrementCounterActivity>())
                .Finalize(),
                When(MessageCreated)
                     .Then(context =>
                     {
                         LogContext.Info?.Log("Message with counter created: {CorrelationId}", context.CorrelationId);
                     })
                .Finalize());

            SetCompletedWhenFinalized();
        }

        public State WaitingForMessageCreation { get; private set; } = null!; 
        public Event<InitSaga> InitSaga { get; private set; } = null!;
        public Event<MessageCreated> MessageCreated { get; private set; } = null!;
        public Event<MessageFailed> MessageFailed { get; private set; } = null!;

        public void CorrelateEvents()
        {
            Event(() => InitSaga, x => x
           .CorrelateById(m => m.CorrelationId ?? new Guid())
           .SelectId(m => m.CorrelationId ?? new Guid())
           .OnMissingInstance(m => m.Discard()));

            Event(() => MessageCreated, x => x
            .CorrelateById(m => m.CorrelationId ?? new Guid())
            .SelectId(m => m.CorrelationId ?? new Guid())
            .OnMissingInstance(m => m.Discard()));

            Event(() => MessageFailed, x => x
            .CorrelateById(m => m.CorrelationId ?? new Guid())
            .SelectId(m => m.CorrelationId ?? new Guid())
            .OnMissingInstance(m => m.Discard()));
        }
    }
}
