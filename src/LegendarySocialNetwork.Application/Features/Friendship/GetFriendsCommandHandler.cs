using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;
using LegendarySocialNetwork.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendarySocialNetwork.Application.Features.Friendship
{
    public record GetFriendsCommandRequest: IRequest<Result<List<FriendEntity>>>;
    public class GetFriendsCommandHandler : IRequestHandler<GetFriendsCommandRequest, Result<List<FriendEntity>>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IFriendshipRepository _friendshipRepository;

        public GetFriendsCommandHandler(ICurrentUserService currentUserService,
            IFriendshipRepository friendshipRepository)
        {
            _currentUserService = currentUserService;
            _friendshipRepository = friendshipRepository;
        }
        public Task<Result<List<FriendEntity>>> Handle(GetFriendsCommandRequest request, CancellationToken cancellationToken)
        {
            return _friendshipRepository.GetFriendsAsync(_currentUserService.GetUserId);
        }
    }
}
