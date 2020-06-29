using System.Collections.Generic;
using Api.Models;
using MongoDB.Driver;
using Tag = Api.Models.Tag;

namespace Api.Services
{
    public class TagService
    {
        private readonly IMongoCollection<Tag> _tags;

        public TagService(IMongoClient client, INewsStoreDatabaseSettings settings)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            _tags = database.GetCollection<Tag>(settings.TagCollectionName);

        }

        public List<Tag> Get() =>
            _tags.Find(tag => true).ToList();

        public Tag Get(string id) =>
            _tags.Find(tag => tag.Id == id).FirstOrDefault();

        public List<Tag> Search(string name) =>
            _tags.Find(Builders<Tag>.Filter.Text(name)).ToList();

        public Tag Create(Tag tag)
        {
            _tags.InsertOne(tag);
            return tag;
        }

        public ICollection<string> Create(IEnumerable<string> tagNames, string articleId)
        {
            var tags = new List<string>();
            foreach (var tagName in tagNames)
            {
                var tag = _tags.Find(Builders<Tag>.Filter.Text(tagName)).FirstOrDefault();
                if (tag == null)
                {
                    var newTag = new Tag(tagName, articleId);
                    _tags.InsertOne(newTag);
                    tag = newTag;
                }
                else
                {
                    tag.Articles.Add(articleId);
                    tag.Count++;
                    Update(tag.Id, tag);
                }

                tags.Add(tag.Id);
            }

            return tags;
        }

        public void Update(string id, Tag tagIn) =>
            _tags.ReplaceOne(tag => tag.Id == id, tagIn);

        public void Remove(Tag tagIn) =>
            _tags.DeleteOne(tag => tag.Id == tagIn.Id);

        public void Remove(string id) =>
            _tags.DeleteOne(tag => tag.Id == id);

        public void Remove(IEnumerable<string> tags)
        {
            foreach (var tagId in tags)
            {
                var tag = _tags.Find(tag => tag.Id == tagId).FirstOrDefault();
                tag.Count--;
                if (tag.Count <= 0)
                {
                    Remove(tagId);
                }
            }
        }
    }
}