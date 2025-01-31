using LegendarySocialNetwork.Application.Features.Chat;
using LegendarySocialNetwork.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegendarySocialNetwork.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ChatController : ApiControllerBase
    {
        [HttpPost("{user_id}/send")]
        public async Task<IActionResult> Send(string user_id, string text)
        {
            var res = await Mediator.Send(new SendCommand(user_id, text));
            if (res.Succeeded)
            {
                return Ok("Успешно отправленно сообщение.");
            }
            return BadRequest(res.Error);
        }

        [HttpGet("{user_id}/list")]
        public async Task<IActionResult> List(string user_id)
        {
            var res = await Mediator.Send(new ListCommand(user_id));
            if (res.Succeeded)
            {
                return Ok(res.Value);
            }
            return BadRequest(res.Error);
        }
    }
}
