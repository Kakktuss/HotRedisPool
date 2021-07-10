using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Redis.Entities;
using StackExchange.Redis;

namespace Redis.Client
{
    public class RedisClient : RedisClient<object>
    {
        public RedisClient(IDatabase database) : base(database)
        {
        }
    }
    
    public class RedisClient<T> : IRedisClient<T>
    {
        private IDatabase _database;

        public RedisClient(IDatabase database)
        {
            _database = database;
        }

        public async Task<T> GetStringKey(string keyName)
        {
            var valueString = (await _database.StringGetAsync(keyName)).ToString();

            if (valueString == null)
            {
                throw new ArgumentException("The request key doesn't exists in the target database");
            }
            
            return JsonSerializer.Deserialize<T>(valueString);
        }

        public Task SaveStringKey(string keyName, T value)
        {
            return _database.StringSetAsync(keyName, JsonSerializer.Serialize(value));
        }

        public async Task<HashEntity<T>> GetHashKey(string keyName)
        {
            var hashEntries = await _database.HashGetAllAsync(keyName);

            var hashEntity =
                new HashEntity<T>(JsonSerializer.Deserialize<T>(hashEntries.FirstOrDefault(e => e.Name == "data").Value));

            hashEntity.CreatedAt = DateTime.Parse(hashEntries.FirstOrDefault(e => e.Name == "createdAt").Value);

            return hashEntity;
        }
        
        public Task SaveHashKey(string keyName, T value)
        {
            var hashEntity = new HashEntity(value);
        
            var hashEntries = new HashEntry[]
            {
                new HashEntry("createdAt", hashEntity.CreatedAt.ToString(CultureInfo.InvariantCulture)),
                new HashEntry("data", JsonSerializer.Serialize(hashEntity.Data))
            };
            
            return _database.HashSetAsync(keyName, hashEntries);
        }
    }
}