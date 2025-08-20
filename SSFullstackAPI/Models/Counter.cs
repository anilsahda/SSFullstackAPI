using MongoDB.Bson.Serialization.Attributes;

namespace SSFullstackAPI.Models
{
    public class Counter
    {
        [BsonId]
        public string Id { get; set; }
        public int Seq { get; set; }
    }
}
