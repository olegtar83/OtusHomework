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
            var res = await Mediator.Send(new SetFriendShipCommandRequest(user_id));
            if (res.Succeeded)
            {
                return Ok("Пользователь успешно указал своего друга");
            }
            return BadRequest(res.Error.ToString());
        }

        [HttpPut("[action]/{user_id}")]
        public async Task<IActionResult> Delete(string user_id)
        {
            var res = await Mediator.Send(new DeleteFriendshipCommandRequest(user_id));
            if (res.Succeeded)
            {
                return Ok("Пользователь успешно удалил из друзей пользователя");
            }
            return BadRequest(res.Error.ToString());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Get()
        {
            var res = await Mediator.Send(new GetFriendsCommandRequest());
            if (res.Succeeded)
            {
                return Ok(res.Value);
            }
            return BadRequest(res.Error.ToString());
        }
    }
}
