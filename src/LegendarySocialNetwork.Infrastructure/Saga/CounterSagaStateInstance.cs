using MassTransit;
namespace LegendarySocialNetwork.Infrastructure.Saga
{
    public class CounterSagaStateInstance : SagaStateMachineInstance, ISagaVersion

    {
        public required string CurrentState { get; set; }
        public DateTime CurrentDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required string To { get; set; }
        public required string From { get; set; }
        // Default props
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
    }
}
