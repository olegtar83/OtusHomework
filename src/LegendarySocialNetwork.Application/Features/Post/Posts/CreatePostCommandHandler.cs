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
        private readonly IPublisher _publisher;

        public CreatePostCommandHandler(IPostRepository postRepository,
            IFriendshipRepository friendshipRepository,
            ICurrentUserService currentUserService,
            IPublisher publisher)
        {
            _currentUserService = currentUserService;
            _postRepository = postRepository;
            _friendshipRepository = friendshipRepository;
            _publisher = publisher;
        }
        public async Task<Result<Unit>> Handle(CreatePostCommandRequest request, CancellationToken cancellationToken)
        {
            var postId = await _postRepository.CreateAsync(request.Text, _currentUserService.GetUserId);

            var friendships = await _friendshipRepository.GetAsync(_currentUserService.GetUserId);

            if (!friendships.Value.Any())
                Result<Unit>.Success(Unit.Value);

            await _publisher.Publish(new UpdateFeedEventRequested(
                new Domain.Messages.UpdateFeedMessage
                {
                    Post = new Domain.Messages.PostMessage
                    {
                        Id = postId.Value,
                        Text = request.Text,
                        UserId = _currentUserService.GetUserId,
                        Created = DateTime.UtcNow
                    },
                    FriendsIds = friendships.Value.Select(x => x.Addressed_id),
                    Operation = Domain.Messages.Operation.Create
                }));

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
