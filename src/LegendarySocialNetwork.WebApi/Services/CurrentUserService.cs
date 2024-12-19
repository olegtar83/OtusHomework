using LegendarySocialNetwork.Application.Common.Interfaces;
using System.Security.Claims;

namespace LegendarySocialNetwork.WebApi.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public string GetUserId
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                    return string.Empty;

                return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? string.Empty;
            }
        }
    }
}
