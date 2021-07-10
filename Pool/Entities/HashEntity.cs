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

            UpdatedAt = DateTime.UtcNow;
        }
        
        public DateTime CreatedAt { get; set; }
        
        public T Data { get; private set; }

        public DateTime UpdatedAt { get; private set; }
        
        public void SetData(T data)
        {
            Data = data;
            
            UpdatedAt = DateTime.UtcNow;
        }
    }
}