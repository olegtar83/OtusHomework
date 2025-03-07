using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.Auth.Login
{
    public class LoginUserQueryRequest : IRequest<Result<AccountEntity>>
    {
        public required string Id { get; set; }
        public required string Password { get; set; }
    }

    public class LoginUserQuery : IRequestHandler<LoginUserQueryRequest, Result<AccountEntity>>
    {
        private readonly IAuthRepository _authRepository;
        public LoginUserQuery(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        public Task<Result<AccountEntity>> Handle(LoginUserQueryRequest request, CancellationToken cancellationToken)
        {
            return _authRepository.GetLoginAsync(request.Id);
        }
    }
}
