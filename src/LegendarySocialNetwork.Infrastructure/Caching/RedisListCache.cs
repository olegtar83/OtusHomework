using CachingFramework.Redis;
using LegendarySocialNetwork.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace LegendarySocialNetwork.Infrastructure.Caching
{
    public class RedisListCache<T> : IListCache<T>
    {
        private readonly ILogger<RedisListCache<T>> _logger;
        private readonly IRedisCacheClient _redisCacheClient;

        public RedisListCache(IRedisCacheClient redisCacheClient, ILogger<RedisListCache<T>> logger)
        {
            _redisCacheClient = redisCacheClient;
            _logger = logger;
        }

        public List<T> GetAsync(string key)
        {
            try
            {
                var context = new RedisContext(_redisCacheClient.GetDbFromConfiguration().Database.Multiplexer);

                var list = context.Collections.GetRedisList<T>(key);

                return list.GetRange().ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Redis exception: {e.Message}");

                return new List<T>();
            }
        }

        public async Task ResetAsync(string key, IEnumerable<T> data)
        {
            try
            {
                var context = new RedisContext(_redisCacheClient.GetDbFromConfiguration().Database.Multiplexer);

                var list = context.Collections.GetRedisList<T>(key);

                await list.ClearAsync();
                await list.AddRangeAsync(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Redis exception: {e.Message}");
            }
        }

        public async Task AddAsync(string key, T data)
        {
            try
            {
                var context = new RedisContext(_redisCacheClient.GetDbFromConfiguration().Database.Multiplexer);

                var list = context.Collections.GetRedisList<T>(key);
                await list.AddAsync(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Redis exception: {e.Message}");
            }
        }
    }
}