using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.Post.Posts
{
    public record GetPostCommandRequest(string PostId) : IRequest<Result<PostEntity>>;
    public class GetPostCommandHandler : IRequestHandler<GetPostCommandRequest, Result<PostEntity>>
    {
        private readonly IPostRepository _postRepository;
        public GetPostCommandHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<Result<PostEntity>> Handle(GetPostCommandRequest request, CancellationToken cancellationToken)
        {
            return await _postRepository.GetAsync(request.PostId);
        }
    }
}
