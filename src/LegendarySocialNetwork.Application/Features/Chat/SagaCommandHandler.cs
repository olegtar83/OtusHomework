using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Application.Events;
using LegendarySocialNetwork.Domain.Messages;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.Chat
{
    public class SagaCommandRequest(string СompanionId, string Text) : IRequest<Result<Unit>>
    {
        public string СompanionId { get; } = СompanionId;
        public string Text { get; } = Text;
    }

    public class SagaCommandHandler : IRequestHandler<SagaCommandRequest, Result<Unit>>
    {
        private readonly IPublisher _publisher;
        private readonly ICurrentUserService _currentUserService;
        public SagaCommandHandler(ICurrentUserService currentUserService, IPublisher publisher)
        {
            _publisher = publisher;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Unit>> Handle(SagaCommandRequest request, CancellationToken cancellationToken)
        {
            var sagaMessage = new InitSaga
            {
                From = _currentUserService.GetUserId,
                To = request.СompanionId,
                Text = request.Text,
            };

            await _publisher.Publish(new InitSagaEventRequested(sagaMessage));

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
