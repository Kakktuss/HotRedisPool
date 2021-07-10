using System;

namespace Redis.Entities
{
    public class HashEntity : HashEntity<object>
    {
        public HashEntity(object data) : base(data) { }
    }
    
    public class HashEntity<T>
    {
        public HashEntity(T data)
        {
            CreatedAt = DateTime.UtcNow;

            Data = data;
        }
        
        public DateTime CreatedAt { get; set; }
        
        public object Data { get; }
    }
}