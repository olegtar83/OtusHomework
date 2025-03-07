using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;
using LegendarySocialNetwork.Infrastructure.Repositories;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.User
{
    public record SearchUsersQueryRequest(string? FirstName = null, string? LastName = null) : IRequest<Result<List<UserEntity>>>;

    public class SearchUsersQueryRequestHandler : IRequestHandler<SearchUsersQueryRequest, Result<List<UserEntity>>>
    {
        private readonly IUserRepository _userRepository;

        public SearchUsersQueryRequestHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public  Task<Result<List<UserEntity>>> Handle(SearchUsersQueryRequest request, CancellationToken cancellationToken)
        {
            return _userRepository.SearchUserAsync(request.FirstName, request.LastName);
        }
    }
}
