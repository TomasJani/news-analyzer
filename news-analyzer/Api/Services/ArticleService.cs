using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using SharedModels.Enums;
using SharedModels.Models;

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

        public async Task<List<Article>> Get()
        {
            var foundArticles = await _articles.FindAsync(article => true);
            var articles = await foundArticles.ToListAsync();
            
            articles = await _authorService.AddAuthors(articles);
            
            articles = await _tagService.AddTags(articles);
            
            return articles;
        }

        public async Task<Article> Get(string id)
        {
            var foundArticle = await _articles.FindAsync(article => article.Id == id);
            return await foundArticle.FirstOrDefaultAsync();
        }

        public async Task<List<Article>> Search(SearchQuery searchQuery)
        {
            List<Article> articles;
            if (string.IsNullOrEmpty(searchQuery.Text))
                articles = await Get();
            else
            {
                var foundArticle = await _articles.FindAsync(Builders<Article>.Filter.Text(searchQuery.Text));
                articles = await foundArticle.ToListAsync();   
            }

            articles = FilterByCategories(articles, searchQuery);

            articles = await _tagService.FilterByTagName(articles, searchQuery.TagName);
            
            articles = await _authorService.FilterByAuthor(articles, searchQuery.AuthorName);

            articles = FilterByDate(articles, searchQuery.StartDate, searchQuery.EndDate);

            articles = await _authorService.AddAuthors(articles);

            articles = await _tagService.AddTags(articles);

            return articles;
        }

        private List<Article> FilterByDate(List<Article> articles, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate == null || endDate == null) 
                return articles;
            return articles.FindAll(article => article.Published > startDate && article.Published < endDate);
        }

        private List<Article> FilterByCategories(List<Article> articles, SearchQuery searchQuery)
        {
            if (searchQuery.SearchForeignNews && searchQuery.SearchHomeNews) 
                return articles;
            if (searchQuery.SearchForeignNews)
                return articles.FindAll(article => article.Category == Category.ForeignNews);
            return searchQuery.SearchHomeNews ? 
                articles.FindAll(article => article.Category == Category.HomeNews) : 
                new List<Article>();
        }


        public async Task<Article> Create(Article article)
        {
            var foundArticle = await _articles.FindAsync(a => a.Title == article.Title);
            if (await foundArticle.FirstOrDefaultAsync() != null)
                return null;
            await _articles.InsertOneAsync(article);
            return article;
        }

        public async Task<Article> Create(RawArticle data)
        {
            var foundArticle = await _articles.FindAsync(a => a.Title == data.Title);
            if (await foundArticle.FirstOrDefaultAsync() != null)
                return null;
            
            var ( title, description, content, authorName, photo, category, 
                site, tagNames, timePublished, url ) = data;
            
            var article = new Article(title, description, content, url, photo, category, timePublished);
            await _articles.InsertOneAsync(article);
            
            var tags = await _tagService.Create(tagNames, article.Id);
            var author = await _authorService.Create(authorName, site, article.Id);

            article.TagsIds = tags;
            article.AuthorId = author?.Id;
            
            await Update(article.Id, article);

            return article;
        }

        public async Task Update(string id, Article articleIn) =>
            await _articles.ReplaceOneAsync(article => article.Id == id, articleIn);

        public async Task Remove(Article articleIn)
        {
            await SafeRemove(articleIn);
            await _articles.DeleteOneAsync(article => article.Id == articleIn.Id);
        }

        public async Task Remove(string id)
        {
            var foundArticle = await _articles.FindAsync(a => a.Id == id);
            var article = await foundArticle.FirstOrDefaultAsync();
            await SafeRemove(article);
            await _articles.DeleteOneAsync(a => a.Id == id);
        }

        public async Task SafeRemove(Article article)
        {
            await _tagService.Remove(article.TagsIds);
            await _authorService.Remove(article.AuthorId);
        }

        public async Task<bool> TitleExists(string title)
        {
            var foundArticle = await _articles.FindAsync(a => a.Title == title);
            return await foundArticle.FirstOrDefaultAsync() != null;
        }
    }
}