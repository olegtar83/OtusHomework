using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Application.Events;
using LegendarySocialNetwork.Infrastructure.Repositories;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.Friendship
{
    public record DeleteFriendshipCommandRequest(string UserId) : IRequest<Result<Unit>>;
    public class DeleteFriendshipCommandHandler : IRequestHandler<DeleteFriendshipCommandRequest, Result<Unit>>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPublisher _publisher;
        public DeleteFriendshipCommandHandler(IFriendshipRepository friendshipRepository,
            ICurrentUserService currentUserService,
            IPublisher publisher)
        {
            _friendshipRepository = friendshipRepository;
            _currentUserService = currentUserService;
            _publisher = publisher;

        }
        public async Task<Result<Unit>> Handle(DeleteFriendshipCommandRequest request, CancellationToken cancellationToken)
        {
            await _friendshipRepository.DeleteAsync(_currentUserService.GetUserId, request.UserId);

            foreach (var participantId in new[] { _currentUserService.GetUserId, request.UserId })
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
