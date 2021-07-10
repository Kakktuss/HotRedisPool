using System;
using System.Collections.Generic;
using System.Linq;
using Redis.Client;
using Redis.Client.Options;
using StackExchange.Redis;

namespace Redis
{
    public class RedisPool : IRedisPool
    {
        private readonly RedisPoolOptions _options;

        private readonly Dictionary<string, ConfigurationOptions> _multiplexerOptionsPool = new();

        private readonly Dictionary<string, ClientConfigurationOptions> _clientConfigurationOptionsPool = new();
        
        private readonly Dictionary<string, Lazy<IConnectionMultiplexer>> _multiplexersPool = new();
        
        private readonly Dictionary<string, Lazy<object>> _clientsPool = new();
        
        public RedisPool(RedisPoolOptions options)
        {
            _options = options;
            
            _buildInternalReferential();
        }

        private void _buildInternalReferential()
        {
            foreach ((string databaseName, ConfigurationOptions configurationOptions) in _options
                .ConnectionMultiplexersPool)
            {
                if (!_multiplexersPool.ContainsKey(databaseName))
                {
                    _multiplexersPool.Add(databaseName, new Lazy<IConnectionMultiplexer>(() => _buildConnectionMultiplexer(databaseName)));
                }

                if (!_multiplexerOptionsPool.ContainsKey(databaseName))
                {
                    _multiplexerOptionsPool.Add(databaseName, configurationOptions);
                }
            }

            foreach ((string databaseName, List<Tuple<Type, ClientConfigurationOptions>> clientTypes) in _options.ClientsPool)
            {
                foreach ((var clientType, var clientConfigurationOptions) in clientTypes)
                {
                    if (clientType.GetInterfaces().Any(e => e.IsGenericType))
                    {
                        
                    }
                    
                    if (!_clientsPool.ContainsKey(clientType.Name))
                    {
                        _clientsPool.Add(clientType.Name, new Lazy<object>(() => _buildRedisClient(databaseName, clientType)));
                    }

                    if (!_clientConfigurationOptionsPool.ContainsKey(clientType.Name))
                    {
                        _clientConfigurationOptionsPool.Add(clientType.Name, clientConfigurationOptions);
                    }
                }
            }
        }

        private IConnectionMultiplexer _buildConnectionMultiplexer(string databaseName)
        {
            if (!_multiplexerOptionsPool.TryGetValue(databaseName, out var multiplexerConfigurationOptions))
            {
                throw new ArgumentException("The requested database name is not found into the multiplexer options pool");
            }

            return ConnectionMultiplexer.Connect(multiplexerConfigurationOptions);
        }

        private object _buildRedisClient(string databaseName, Type clientType)
        {
            if (!_multiplexersPool.TryGetValue(databaseName, out var multiplexer))
            {
                throw new ArgumentException("The requested database name is not found into the multiplexers pool");
            }

            if (!_clientConfigurationOptionsPool.TryGetValue(clientType.Name, out var clientConfigurationOptions))
            {
                throw new ArgumentException("The requested client configuration option is not found into the client configuration options pool");
            }

            var multiplexerDatabase = multiplexer.Value.GetDatabase(clientConfigurationOptions.Database);
            
            return Activator.CreateInstance(typeof(RedisClient<>).MakeGenericType(clientType), new object[] { multiplexerDatabase });
        }
        
        public IRedisClient<T> GetClient<T>()
        {
            if (_clientsPool.TryGetValue(typeof(T).Name, out var client))
            {
                return (IRedisClient<T>) client.Value;
            }

            throw new ArgumentException("The requested type is not found in the pool");
        }
    }
}