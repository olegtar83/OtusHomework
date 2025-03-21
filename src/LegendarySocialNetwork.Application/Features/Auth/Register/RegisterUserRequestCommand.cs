﻿using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.Auth.Register
{
    public class RegisterUserRequestCommand : IRequestHandler<RegisterUserRequest, Result<string>>
    {
        private readonly IAuthRepository _authRepository;

        public RegisterUserRequestCommand(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public Task<Result<string>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            return _authRepository.RegisterAsync(new Domain.Entities.UserEntity
            {
                Age = request.Age,
                Biography = request.Biography,
                City = request.City,
                First_name = request.First_name,
                Id = request.Id,
                Second_name = request.Second_name,
                Sex = request.Sex
            }, request.Password);
        }
    }
}
