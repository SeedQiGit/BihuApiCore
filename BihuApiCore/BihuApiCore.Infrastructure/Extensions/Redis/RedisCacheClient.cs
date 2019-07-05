using StackExchange.Redis;
using System;
using System.Collections.Concurrent;


namespace BihuApiCore.Infrastructure.Extensions
{
    public class RedisCacheClient : IDisposable
    {
        private readonly string _connectionString; 
        private readonly string _instanceName; 
        private readonly int _defaultDb;
        private ConcurrentDictionary<string, ConnectionMultiplexer> _connections;

        public RedisCacheClient(string connectionString, string instanceName, int defaultDb = 0)
        {
            _connectionString = connectionString;
            _instanceName = instanceName;
            _defaultDb = defaultDb;
            _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        }

        private ConnectionMultiplexer GetConnect()
        {
            return _connections.GetOrAdd(_instanceName, p => ConnectionMultiplexer.Connect(_connectionString));
        }       
        public IDatabase GetDatabase()
        {
            return GetConnect().GetDatabase(_defaultDb);
        }

        public IServer GetServer(string configName = null, int endPointsIndex = 0)
        {
            var confOption = ConfigurationOptions.Parse(_connectionString);
            return GetConnect().GetServer(confOption.EndPoints[endPointsIndex]);
        }

        public ISubscriber GetSubscriber(string configName = null)
        {
            return GetConnect().GetSubscriber();
        }

        public void Dispose()
        {
            if (_connections != null && _connections.Count > 0)
            {
                foreach (var item in _connections.Values)
                {
                    item.Close();
                }
            }
        }
    }
}
