using System;
using System.Collections.Generic;
using Redis.Client;
using Redis.Client.Options;
using StackExchange.Redis;

namespace Redis
{
    public class RedisPoolOptions
    {

        public RedisPoolOptions(Dictionary<string, List<Tuple<Type, ClientConfigurationOptions>>> clientsPool,
            Dictionary<string, ConfigurationOptions> connectionMultiplexersPool)
        {
            ClientsPool = clientsPool;

            ConnectionMultiplexersPool = connectionMultiplexersPool;
        }
        
        internal Dictionary<string, List<Tuple<Type, ClientConfigurationOptions>>> ClientsPool { get; }
        
        internal Dictionary<string, ConfigurationOptions> ConnectionMultiplexersPool { get; }
        
    }
}