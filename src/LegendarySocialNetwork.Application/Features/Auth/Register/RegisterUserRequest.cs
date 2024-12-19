using LegendarySocialNetwork.Application.Common.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace LegendarySocialNetwork.Application.Features.Auth.Register
{
    public class RegisterUserRequest : IRequest<Result<string>>
    {
        public required string First_name { get; set; }

        public required string Second_name { get; set; }

        public required int Age { get; set; }

        public required string Sex { get; set; }

        public required string Biography { get; set; }

        public required string City { get; set; }

        [MinLength(6)]
        public required string Password { get; set; }
    }
}
