using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Configuration;
using Redis.Client;
using Redis.Client.Options;
using Redis.Configuration;
using StackExchange.Redis;

namespace Redis
{
    public class RedisPoolOptionsBuilder
    {
        private Dictionary<string, Type> _typesToBind = new();
        
        private RedisParameters _redisParameters;

        public RedisPoolOptionsBuilder()
        {
            _redisParameters = new();
        }

        public RedisPoolOptionsBuilder Bind<T>()
        {
            if (!_typesToBind.ContainsKey(typeof(T).Name))
            {
                _typesToBind.Add(typeof(T).Name, typeof(T));
            }
            
            return this;
        }

        public RedisPoolOptionsBuilder WithConfig(IConfigurationSection configurationSection)
        {
            configurationSection.Bind(_redisParameters);

            return this;
        }

        internal RedisPoolOptions Build()
        {
            var clients = new Dictionary<string, List<Tuple<Type, ClientConfigurationOptions>>>();

            var multiplexers = new Dictionary<string, ConfigurationOptions>();

            foreach (RedisRoutings redisRoutings in _redisParameters.RedisRoutings)
            {
                var clientsMap = new List<Tuple<Type, ClientConfigurationOptions>>();

                foreach (RedisEntity redisEntity in redisRoutings.Entities)
                {
                    if (!_typesToBind.ContainsKey(redisEntity.Type))
                    {
                        throw new ArgumentException($"The type {redisEntity.Type} is not bound. Add options.Bind<{redisEntity.Type}>() into AddRedisClientPool");
                    }
                    
                    var clientType = _typesToBind[redisEntity.Type];

                    var clientOptions = new ClientConfigurationOptions
                    {
                        Database = redisEntity.Db
                    };
                    
                    clientsMap.Add(new Tuple<Type, ClientConfigurationOptions>(clientType, clientOptions));
                }
                
                clients.Add(redisRoutings.ConnectionString, clientsMap);
                
                var multiplexerOptions = ConfigurationOptions.Parse(redisRoutings.ConnectionString);
                
                multiplexers.Add(redisRoutings.ConnectionString, multiplexerOptions);
            }

            return new RedisPoolOptions(clients, multiplexers);
        }
    }
}