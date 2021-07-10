using System;
using Microsoft.Extensions.DependencyInjection;

namespace Redis.Extensions
{
    public static class DependencyInjectionExtensions
    {

        public static void AddRedisClientPool(this IServiceCollection services, Action<RedisPoolOptionsBuilder> options)
        {
            RedisPoolOptionsBuilder redisPoolOptionsBuilder = new();
            
            options(redisPoolOptionsBuilder);

            services.AddSingleton(redisPoolOptionsBuilder.Build());
            services.AddSingleton<IRedisPool, RedisPool>();
        }
        
    }
}