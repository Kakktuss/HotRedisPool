using Redis.Client;

namespace Redis
{
    public interface IRedisPool
    {
        public IRedisClient<T> GetClient<T>();
    }
}