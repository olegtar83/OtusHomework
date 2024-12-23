using LegendarySocialNetwork.Messages.DataClasses.Requests;
using LegendarySocialNetwork.Messages.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LegendarySocialNetwork.Messages.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class DialogController : ControllerBase
    {
        private readonly IDialogService _dialogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DialogController(IDialogService dialogService,
            IHttpContextAccessor httpContextAccessor)
        {
            _dialogService = dialogService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("{user_id}/[action]")]
        public async Task<IActionResult> Send(string user_id, DialogReq req)
        {
            var res = await _dialogService.SetDialogAsync(GetUserId, user_id, req.Text);
            if (res.Succeeded)
            {
                return Ok("Успешно отправленно сообщение.");
            }
            return BadRequest(res.Error);
        }

        [HttpGet("{user_id}/[action]")]
        public async Task<IActionResult> List(string user_id)
        {
            var res = await _dialogService.GetDialogsAsync(user_id);
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
