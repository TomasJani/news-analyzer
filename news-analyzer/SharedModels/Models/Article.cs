using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SharedModels.Enums;

namespace SharedModels.Models
{
    public class Article
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public string Photo { get; set; }
        public Category Category { get; set; }
        public DateTime Published { get; set; }

        public string AuthorId { get; set; }

        public ICollection<string> Tags { get; set; }

        public Article(string title, string description, string content, string url, string photo, Category category,
            DateTime published)
        {
            Title = title;
            Description = description;
            Content = content;
            Url = url;
            Photo = photo;
            Category = category;
            Published = published;
        }
    }
}