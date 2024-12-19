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

            var friendships = await _friendshipRepository.GetAsync(_currentUserService.GetUserId);

            await _publisher.Publish(new UpdateFeedEventRequested(
                new Domain.Messages.UpdateFeedMessage
                {
                    Operation = Domain.Messages.Operation.Delete,
                    Post = new Domain.Messages.PostMessage
                    {
                        Id = request.PostId,
                        Text = string.Empty,
                        UserId = _currentUserService.GetUserId
                    },
                    FriendsIds = friendships.Value.Select(x => x.Addressed_id)
                }
                ));
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
