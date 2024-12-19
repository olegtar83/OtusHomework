using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Application.Events;
using LegendarySocialNetwork.Infrastructure.Repositories;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.Friendship
{
    public record SetFriendShipCommandRequest(string userId) : IRequest<Result<Unit>>;
    public class SetFriendShipCommandHandler : IRequestHandler<SetFriendShipCommandRequest, Result<Unit>>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPublisher _publisher;
        public SetFriendShipCommandHandler(IFriendshipRepository friendshipRepository,
            ICurrentUserService currentUserService,
            IPublisher publisher)
        {
            _friendshipRepository = friendshipRepository;
            _currentUserService = currentUserService;
            _publisher = publisher;

        }

        public async Task<Result<Unit>> Handle(SetFriendShipCommandRequest request, CancellationToken cancellationToken)
        {
            await _friendshipRepository.SetAsync(_currentUserService.GetUserId, request.userId);

            foreach (var participantId in new[] { _currentUserService.GetUserId, request.userId })
            {
                var message = new Domain.Messages.UpdateFeedMessage
                {
                    Operation = Domain.Messages.Operation.Reset,
                    UserId = participantId
                };
                await _publisher.Publish(new UpdateFeedEventRequested(message));
            }
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
