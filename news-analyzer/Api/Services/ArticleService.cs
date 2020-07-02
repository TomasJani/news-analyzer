using System.Collections.Generic;
using Api.Models;
using Data;
using MongoDB.Driver;

namespace Api.Services
{
    public class ArticleService
    {
        private readonly IMongoCollection<Article> _articles;
        private readonly TagService _tagService;
        private readonly AuthorService _authorService;

        public ArticleService(IMongoClient client, INewsStoreDatabaseSettings settings, TagService tagService, AuthorService authorService)
        {
            _tagService = tagService;
            _authorService = authorService;
            
            var database = client.GetDatabase(settings.DatabaseName);

            _articles = database.GetCollection<Article>(settings.ArticleCollectionName);
        }

        public List<Article> Get() =>
            _articles.Find(article => true).ToList();

        public Article Get(string id) =>
            _articles.Find(article => article.Id == id).FirstOrDefault();

        public List<Article> Search(string text) =>
            _articles.Find(Builders<Article>.Filter.Text(text)).ToList();

        public Article Create(Article article)
        {
            if (_articles.Find(a => a.Title == article.Title).FirstOrDefault() != null)
                return null;
            _articles.InsertOne(article);
            return article;
        }

        public Article Create(RawArticle data)
        {
            if (_articles.Find(a => a.Title == data.Title).FirstOrDefault() != null)
                return null;
            
            var ( title, description, content, authorName, photo, category, 
                site, tagNames, timePublished, url ) = data;
            
            var article = new Article(title, description, content, url, photo, category, timePublished);
            _articles.InsertOne(article);
            
            var tags = _tagService.Create(tagNames, article.Id);
            var author = _authorService.Create(authorName, site, article.Id);

            article.Tags = tags;
            article.AuthorId = author?.Id;
            
            Update(article.Id, article);

            return article;
        }

        public void Update(string id, Article articleIn) =>
            _articles.ReplaceOne(article => article.Id == id, articleIn);

        public void Remove(Article articleIn)
        {
            SafeRemove(articleIn);
            _articles.DeleteOne(article => article.Id == articleIn.Id);
        }

        public void Remove(string id)
        {
            var article = _articles.Find(a => a.Id == id).FirstOrDefault();
            SafeRemove(article);
            _articles.DeleteOne(a => a.Id == id);
        }

        public void SafeRemove(Article article)
        {
            _tagService.Remove(article.Tags);
            _authorService.Remove(article.AuthorId);
        }

        public bool TitleExists(string title)
        {
            return _articles.Find(a => a.Title == title).FirstOrDefault() != null;
        }
    }
}