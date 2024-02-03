
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;
using WebAPICachingWithRedis.Application.Interfaces;
using WebAPICachingWithRedis.Configurations;

namespace WebAPICachingWithRedis.Application.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly RedisConfiguration _configurationMonitor;
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public CacheService(IOptionsMonitor<RedisConfiguration> configurationMonitor)
        {
            _configurationMonitor = configurationMonitor.CurrentValue;
            _connectionMultiplexer = Connect();
            _database = Database(db: _configurationMonitor.Database, connectionMultiplexer: _connectionMultiplexer);
        }

        public ConnectionMultiplexer Connect()
        {
            try
            {
                ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(_configurationMonitor.RedisConnectionString);
                RegisterEvents();

                return connectionMultiplexer;
            }
            catch (Exception exception)
            {

                throw new Exception(exception.Message);
            }
        }
        public IDatabase Database(int db, IConnectionMultiplexer connectionMultiplexer)
        {
            IDatabase database = connectionMultiplexer.GetDatabase(db);
            return database;
        }
        public T GetCache<T>(string key)
        {


            RedisValue cacheData = _database.StringGet(key);
            if (!string.IsNullOrEmpty(cacheData))
                return JsonSerializer.Deserialize<T>(cacheData);
            else
                return default;
        }

        public void RemoveCache(string key)
        {

            var exist = _database.KeyExists(key);
            if (exist)
                _database.KeyDelete(key);
            else
                return;
        }
        public void SetCache<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);
            _database.StringSet(key, JsonSerializer.Serialize<T>(value), expirtyTime);
        }


        private void RegisterEvents()
        {
            _connectionMultiplexer.ConnectionRestored += _connectionMultiplexer_ConnectionRestored;
            _connectionMultiplexer.ErrorMessage += _connectionMultiplexer_ErrorMessage;
            _connectionMultiplexer.ConnectionFailed += _connectionMultiplexer_ConnectionFailed;
        }



        private void _connectionMultiplexer_ConnectionRestored(object? sender, ConnectionFailedEventArgs e)
        {
            //Connection Restored
        }

        private void _connectionMultiplexer_ConnectionFailed(object? sender, ConnectionFailedEventArgs e)
        {
            //Connection Faild
        }

        private void _connectionMultiplexer_ErrorMessage(object? sender, RedisErrorEventArgs e)
        {
            //Exception Message
        }
    }
}
