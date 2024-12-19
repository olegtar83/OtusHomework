namespace LegendarySocialNetwork.Application.Common.Interfaces
{
    public interface IListCache<T>
    {
        List<T> GetAsync(string key);
        Task ResetAsync(string key, IEnumerable<T> data);
        Task AddAsync(string key, T data);
    }
}