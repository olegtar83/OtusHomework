using LegendarySocialNetwork.Application.Features.Friendship;
using LegendarySocialNetwork.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegendarySocialNetwork.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FriendsController : ApiControllerBase
    {
        [HttpPut("[action]/{user_id}")]
        public async Task<IActionResult> Set(string user_id)
        {
            await Mediator.Send(new SetFriendShipCommandRequest(user_id));
            return Ok("Пользователь успешно указал своего друга");
        }

        [HttpPut("[action]/{user_id}")]
        public async Task<IActionResult> Delete(string user_id)
        {
            await Mediator.Send(new DeleteFriendshipCommandRequest(user_id));
            return Ok("Пользователь успешно удалил из друзей пользователя");
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> Get()
        {
            await Mediator.Send(new GetFriendsCommandRequest());
            return Ok("Пользователь успешно удалил из друзей пользователя");
        }
    }
}
