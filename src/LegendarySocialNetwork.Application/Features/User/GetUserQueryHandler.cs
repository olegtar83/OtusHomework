using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;
using LegendarySocialNetwork.Infrastructure.Repositories;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.User
{
    public record GetUserQueryRequest(string UserId) : IRequest<Result<UserEntity>>;

    public class GetUserQueryHandler : IRequestHandler<GetUserQueryRequest, Result<UserEntity>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public Task<Result<UserEntity>> Handle(GetUserQueryRequest request, CancellationToken cancellationToken)
        {
            return _userRepository.GetUserAsync(request.UserId);
        }
    }
}
