using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Application.Events;
using LegendarySocialNetwork.Infrastructure.Repositories;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.Post.Posts
{
    public record CreatePostCommandRequest(string Text) : IRequest<Result<Unit>>;
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommandRequest, Result<Unit>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IPublisher _publisher;

        public CreatePostCommandHandler(IPostRepository postRepository,
            IFriendshipRepository friendshipRepository,
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            IPublisher publisher)
        {
            _currentUserService = currentUserService;
            _postRepository = postRepository;
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
            _publisher = publisher;
        }
        public async Task<Result<Unit>> Handle(CreatePostCommandRequest request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId;
            var postId = await _postRepository.CreateAsync(request.Text, userId);
            var friendshipsResult = await _friendshipRepository.GetAsync(userId);

            if (!friendshipsResult.Value.Any())
                return Result<Unit>.Success(Unit.Value);

            var postMessage = new Domain.Messages.PostMessage
            {
                Id = postId.Value,
                Text = request.Text,
                UserId = userId,
                Created = DateTime.UtcNow,
                Name = _currentUserService.GetUserName
            };

            var updateFeedMessage = new Domain.Messages.UpdateFeedMessage
            {
                Post = postMessage,
                FriendsIds = friendshipsResult.Value.Select(x => x.Requester_id == userId
                ? x.Addressed_id : x.Requester_id),
                Operation = Domain.Messages.Operation.Create
            };

            await _publisher.Publish(new UpdateFeedEventRequested(updateFeedMessage));

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
