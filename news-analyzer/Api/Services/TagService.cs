using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SharedModels.Models;
using Tag = SharedModels.Models.Tag;

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

        public async Task<List<Tag>> Get()
        {
            var foundTag = await _tags.FindAsync(tag => true);
            return await foundTag.ToListAsync();
        }

        public async Task<Tag> Get(string id)
        {
            var foundTag = await _tags.FindAsync(tag => tag.Id == id);
            return await foundTag.FirstOrDefaultAsync();
        }

        public async Task<List<Tag>> Search(string name)
        {
            var foundTag = await _tags.FindAsync(Builders<Tag>.Filter.Text(name));
            return await foundTag.ToListAsync();
        }

        public async Task<List<Article>> FilterByTagName(List<Article> articles, string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) 
                return articles;

            var tagSearch = await Search(tagName);
            var tags = tagSearch.Select(tag => tag.Id).ToList();
            return articles.Where(article => article.TagsIds.Intersect(tags).Count() != 0).ToList();
        }

        public async Task<Tag> Create(Tag tag)
        {
            await _tags.InsertOneAsync(tag);
            return tag;
        }

        public async Task<ICollection<string>> Create(IEnumerable<string> tagNames, string articleId)
        {
            var tags = new List<string>();
            foreach (var tagName in tagNames)
            {
                if (string.IsNullOrEmpty(tagName)) 
                    continue;

                var foundTag = await _tags.FindAsync(Builders<Tag>.Filter.Text(tagName));
                var tag = await foundTag.FirstOrDefaultAsync();
                if (tag == null)
                {
                    var newTag = new Tag(tagName, articleId);
                    await _tags.InsertOneAsync(newTag);
                    tag = newTag;
                }
                else
                {
                    tag.Articles.Add(articleId);
                    tag.Count++;
                    await Update(tag.Id, tag);
                }

                tags.Add(tag.Id);
            }

            return tags;
        }

        public async Task Update(string id, Tag tagIn) =>
            await _tags.ReplaceOneAsync(tag => tag.Id == id, tagIn);

        public async Task Remove(Tag tagIn) =>
            await _tags.DeleteOneAsync(tag => tag.Id == tagIn.Id);

        public async Task Remove(string id) =>
            await _tags.DeleteOneAsync(tag => tag.Id == id);

        public async Task Remove(IEnumerable<string> tags)
        {
            foreach (var tagId in tags)
            {
                var foundTag = await _tags.FindAsync(t => t.Id == tagId);
                var tag = await foundTag.FirstOrDefaultAsync();
                tag.Count--;
                if (tag.Count <= 0)
                    await Remove(tagId);
            }
        }

        public async Task<bool> NameExists(string name)
        {
            var foundTag = await _tags.FindAsync(t => t.Name == name);
            return await foundTag.FirstOrDefaultAsync() != null;
        }
        
        public async Task<List<Article>> AddTags(List<Article> articles)
        {
            foreach (var article in articles)
            {
                article.Tags = new List<Tag>();
                foreach (var tagsId in article.TagsIds)
                {
                    article.Tags.Add(await Get(tagsId));
                }
            }

            return articles;
        }
    }
}