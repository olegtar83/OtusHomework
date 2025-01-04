using LegendarySocialNetwork.Domain.Messages;

namespace LegendarySocialNetwork.Application.Common.Interfaces
{
    public interface IPushHubNotifier
    {
        Task PublishAsync(PostMessage meesage, string userId);
    }
}
