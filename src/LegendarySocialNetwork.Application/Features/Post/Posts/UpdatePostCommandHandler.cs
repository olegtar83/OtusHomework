using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Application.Events;
using LegendarySocialNetwork.Infrastructure.Repositories;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.Post.Posts
{
    public record UpdatePostCommandRequest(string Text, string PostId) : IRequest<Result<Unit>>;
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommandRequest, Result<Unit>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPublisher _publisher;

        public UpdatePostCommandHandler(IPostRepository postRepository,
            IFriendshipRepository friendshipRepository,
            ICurrentUserService currentUserService,
            IPublisher publisher)
        {
            _currentUserService = currentUserService;
            _friendshipRepository = friendshipRepository;
            _postRepository = postRepository;
            _publisher = publisher;
        }
        public async Task<Result<Unit>> Handle(UpdatePostCommandRequest request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId;

            await _postRepository.UpdateAsync(request.Text, request.PostId, userId);

            var friendshipsResult = await _friendshipRepository.GetAsync(userId);

            if (!friendshipsResult.Value.Any())
                return Result<Unit>.Success(Unit.Value);

            var postMessage = new Domain.Messages.PostMessage
            {
                Id = request.PostId,
                Text = request.Text,
                UserId = userId,
                Created = DateTime.UtcNow
            };

            var updateFeedMessage = new Domain.Messages.UpdateFeedMessage
            {
                Operation = Domain.Messages.Operation.Update,
                Post = postMessage,
                FriendsIds = friendshipsResult.Value.Select(x => x.Requester_id == userId
                            ? x.Addressed_id : x.Requester_id),
            };

            await _publisher.Publish(new UpdateFeedEventRequested(updateFeedMessage));

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
