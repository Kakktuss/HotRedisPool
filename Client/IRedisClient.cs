using System;
using System.Threading.Tasks;
using Redis.Entities;
using StackExchange.Redis;

namespace Redis.Client
{
    public interface IRedisClient : IRedisClient<object>
    {
        
    }
    
    public interface IRedisClient<T>
    {
        public Task<T> GetStringKey(string keyName);

        public Task SaveStringKey(string keyName, T value, TimeSpan expireAt);

        public Task<HashEntity<T>> GetHashKey(string keyName);

        public Task SaveHashKey(string keyName, T value, TimeSpan expireAt);

        public Task UpdateHashKey(string keyName, HashEntity<T> hashEntity);

        public Task<bool> CheckKeyExists(string keyName);

        public Task<bool> CheckKeyHashFieldExists(string keyName, string fieldName);
    }
}