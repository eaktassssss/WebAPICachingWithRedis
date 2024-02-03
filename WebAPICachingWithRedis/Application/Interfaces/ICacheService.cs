using StackExchange.Redis;

namespace WebAPICachingWithRedis.Application.Interfaces
{
    public interface ICacheService
    {
        void RemoveCache(string key);
        T GetCache<T>(string key);
        void SetCache<T>(string key, T value, DateTimeOffset expirationTime);
         ConnectionMultiplexer Connect();
        IDatabase Database(int db ,IConnectionMultiplexer connectionMultiplexer);
    }
}
