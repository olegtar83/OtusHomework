using Asp.Versioning;
using LegendarySocialNetwork.Messages.DataClasses.Requests;
using LegendarySocialNetwork.Messages.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LegendarySocialNetwork.Messages.Controllers
{
    [ApiVersion(1)]
    [ApiVersion(2)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class DialogController : ControllerBase
    {
        private readonly IDialogService _dialogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DialogController> _logger;
        public DialogController(IDialogService dialogService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DialogController> logger)
        {
            _dialogService = dialogService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [MapToApiVersion(1)]
        [HttpPost("{user_id}/send")]
        public async Task<IActionResult> SendV1(string user_id, DialogReq req)
        {
            var res = await _dialogService.SetDialogAsync(GetUserId, user_id, req.Text);
            if (res.Succeeded)
            {
                return Ok("Успешно отправленно сообщение.");
            }
            return BadRequest(res.Error);
        }

        [MapToApiVersion(1)]
        [HttpGet("{user_id}/list")]
        public async Task<IActionResult> ListV1(string user_id)
        {
            LogRequestHeader();
            var res = await _dialogService.GetDialogsAsync(user_id);
            if (res.Succeeded)
            {
                return Ok(res.Value);
            }
            return BadRequest(res.Error);
        }

        [MapToApiVersion(2)]
        [HttpPost("{user_id}/send")]
        public async Task<IActionResult> SendV2(string user_id, DialogReq req)
        {
            var res = await _dialogService.SetDialogAsync(GetUserId, user_id, req.Text);
            if (res.Succeeded)
            {
                return Ok("Успешно отправленно сообщение.");
            }
            return BadRequest(res.Error);
        }

        [MapToApiVersion(2)]
        [HttpGet("{user_id}/list")]
        public async Task<IActionResult> ListV2(string user_id)
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

        private void LogRequestHeader()
        {
            var requestIdHeader = _httpContextAccessor.HttpContext?
                .Request.Headers["X-RequestId"].FirstOrDefault();

            if (requestIdHeader != null)
            {
                _logger.LogInformation($"X-RequestId from header: { requestIdHeader.ToString()}");
            }
        }
    }
}
