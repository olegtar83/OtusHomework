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
            await _postRepository
               .UpdateAsync(
               request.Text,
               request.PostId,
               _currentUserService.GetUserId);

            var friendships = await _friendshipRepository.GetAsync(_currentUserService.GetUserId);

            await _publisher.Publish(new UpdateFeedEventRequested(
                new Domain.Messages.UpdateFeedMessage
                {
                    Operation = Domain.Messages.Operation.Update,
                    Post = new Domain.Messages.PostMessage
                    {
                        Id = request.PostId,
                        Text = request.Text,
                        UserId = _currentUserService.GetUserId,
                        Created = DateTime.UtcNow
                    },
                    FriendsIds = friendships.Value.Select(x => x.Addressed_id)
                }
            ));
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
