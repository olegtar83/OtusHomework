using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.Post.Feed
{
    public record GetFeedCommandRequest(string? id) : IRequest<Result<List<PostEntity>>>;


    public record GetFeedCommandRequestHandler : IRequestHandler<GetFeedCommandRequest, Result<List<PostEntity>>>
    {
        private readonly IFeedRepository _feedRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetFeedCommandRequestHandler(IFeedRepository feedRepository,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _feedRepository = feedRepository;
        }
        public Task<Result<List<PostEntity>>> Handle(GetFeedCommandRequest request, CancellationToken cancellationToken)
        {
            return _feedRepository.GetFeedAsync(request.id ?? _currentUserService.GetUserId);
        }
    }

}
