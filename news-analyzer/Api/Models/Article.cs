using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.DataService.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Models
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
        [DataType(DataType.Date)] public DateTime Published { get; set; }

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