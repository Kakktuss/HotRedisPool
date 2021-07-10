using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Redis.Configuration
{
    public class RedisParameters
    {
        [JsonPropertyName("redisRoutings")]
        public List<RedisRoutings> RedisRoutings { get; set; }
    }

    public class RedisRoutings
    {
        [JsonPropertyName("connectionString")]
        public string ConnectionString { get; set; }
        
        [JsonPropertyName("entities")]
        public List<RedisEntity> Entities { get; set; }
    }

    public class RedisEntity
    {
        [JsonPropertyName("db")]
        public int Db { get; set; }
        
        [JsonPropertyName("Type")]
        public string Type { get; set; }
    }
}