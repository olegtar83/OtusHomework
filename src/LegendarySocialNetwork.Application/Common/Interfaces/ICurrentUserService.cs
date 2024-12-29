namespace LegendarySocialNetwork.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string GetUserId { get; }
        string GetUserName { get; }
    }

}
