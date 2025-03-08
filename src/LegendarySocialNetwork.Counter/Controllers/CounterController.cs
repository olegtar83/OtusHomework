using LegendarySocialNetwork.Counter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LegendarySocialNetwork.Counter.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CounterController : ControllerBase
    {
        private readonly ITarantoolService _tarantoolService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CounterController(ITarantoolService tarantoolService, IHttpContextAccessor httpContextAccessor)
        {
            _tarantoolService = tarantoolService;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpGet("/{user_id}")]
        public async Task<IActionResult> GetAll(string user_id)
        {
            var res = await _tarantoolService.GetMessageCounter(GetUserId, user_id);
            if (res.Succeeded)
            {
                return Ok(res.Value);
            }
            return BadRequest(res.Error);
        }

        private string GetUserId
        {
            get
            {
                var currentUserId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
                ArgumentNullException.ThrowIfNull(nameof(currentUserId));
                return currentUserId!;
            }
        }
    }
}
