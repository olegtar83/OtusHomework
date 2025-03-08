using LegendarySocialNetwork.Domain.Messages;
using LegendarySocialNetwork.Messages.Services;
using MassTransit;

namespace LegendarySocialNetwork.Messages.Consumers
{
    public class CreateMessageConsumer : IConsumer<CreateMessage>
    {
        private readonly IDialogService _dialogService;
        private readonly ILogger<CreateMessageConsumer> _logger;
        private readonly ITopicProducer<string, MessageCreated> _createdProducer;
        private readonly ITopicProducer<string, MessageFailed> _failedProducer;

        public CreateMessageConsumer(IDialogService dialogService,
            ITopicProducer<string, MessageCreated> createdProducer,
            ITopicProducer<string, MessageFailed> failedProducer,
            ILogger<CreateMessageConsumer> logger)
        {
            _dialogService = dialogService;
            _createdProducer = createdProducer;
            _failedProducer = failedProducer;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateMessage> context)
        {
            bool success = new Random().Next(0, 2) == 0;// Randomly succeed or fail

            if (success)
            {
                await _dialogService.SetDialogAsync(context.Message.From, context.Message.To, context.Message.Text);
                await _createdProducer.Produce(Guid.NewGuid().ToString(), new MessageCreated());
            }
            else
            {
                await _failedProducer.Produce(Guid.NewGuid().ToString(), new MessageFailed
                {
                    From = context.Message.From,
                    Text = context.Message.Text,
                    To = context.Message.To,
                });
            }
            _logger.LogInformation($"Consumer messages ended with {success}");
        }
    }
}
