using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.Models
{
    public class Tag
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        
        public ICollection<string> Articles { get; set; }

        [JsonConstructor]
        public Tag(string name)
        {
            Name = name;
            Count = 1;
        }
        
        public Tag(string name, string firstArticleId)
        {
            Name = name;
            Count = 1;
            Articles = new List<string> {firstArticleId};
        }
    }
}