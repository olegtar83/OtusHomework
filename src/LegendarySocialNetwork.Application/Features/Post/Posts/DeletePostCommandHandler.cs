using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Application.Events;
using LegendarySocialNetwork.Infrastructure.Repositories;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.Post.Posts
{
    public record DeletePostCommandRequest(string PostId) : IRequest<Result<Unit>>;
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommandRequest, Result<Unit>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IPublisher _publisher;
        private readonly ICurrentUserService _currentUserService;

        public DeletePostCommandHandler(IPostRepository postRepository,
            IFriendshipRepository friendshipRepository,
            IPublisher publisher,
            ICurrentUserService currentUserService)
        {
            _postRepository = postRepository;
            _friendshipRepository = friendshipRepository;
            _publisher = publisher;
            _currentUserService = currentUserService;
        }
        public async Task<Result<Unit>> Handle(DeletePostCommandRequest request, CancellationToken cancellationToken)
        {
            await _postRepository.DeleteAsync(request.PostId);

            var userId = _currentUserService.GetUserId;
            var friendshipsResult = await _friendshipRepository.GetAsync(userId);

            if (!friendshipsResult.Value.Any())
                return Result<Unit>.Success(Unit.Value);

            var postMessage = new Domain.Messages.PostMessage
            {
                Id = request.PostId,
                Text = string.Empty,
                UserId = userId
            };

            var updateFeedMessage = new Domain.Messages.UpdateFeedMessage
            {
                Operation = Domain.Messages.Operation.Delete,
                Post = postMessage,
                FriendsIds = friendshipsResult.Value.Select(x => x.Requester_id == userId
                            ? x.Addressed_id : x.Requester_id),
            };

            await _publisher.Publish(new UpdateFeedEventRequested(updateFeedMessage));

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
