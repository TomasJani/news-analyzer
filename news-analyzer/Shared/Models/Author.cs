using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Shared.Enums;

namespace Shared.Models
{
    public class Author
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public Site Site { get; set; }
        public int Count { get; set; }
        
        public ICollection<string> Articles { get; set; }
        
        [JsonConstructor]
        public Author(string name, Site site)
        {
            Name = name;
            Site = site;
            Count = 1;
        }

        public Author(string name, Site site, string firstArticleId)
        {
            Name = name;
            Site = site;
            Count = 1;
            Articles = new List<string> {firstArticleId};
        }
    }
}