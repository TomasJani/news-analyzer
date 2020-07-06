using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharedModels.Enums;

namespace SharedModels.Models
{
    public class RawArticle
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string Photo { get; set; }

        [JsonConverter(typeof(JsonConverters.CategoryConverter))]
        public Category Category { get; set; }

        public string Url { get; set; }

        [JsonConverter(typeof(JsonConverters.SiteConverter))]
        public Site Site { get; set; }

        [JsonConverter(typeof(JsonConverters.TagsConverter))]
        public ICollection<string> Tags { get; set; }

        [JsonProperty("time_published")]
        [JsonConverter(typeof(JsonConverters.DateConverter))]
        public DateTime TimePublished { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);

        public void Deconstruct(out string title, out string description, out string content, out string authorName,
            out string photo, out Category category, out Site site, out ICollection<string> tags,
            out DateTime timePublished, out string url)
        {
            title = Title;
            description = Description;
            content = Content;
            authorName = Author;
            photo = Photo;
            category = Category;
            site = Site;
            tags = Tags;
            timePublished = TimePublished;
            url = Url;
        }
    }
}