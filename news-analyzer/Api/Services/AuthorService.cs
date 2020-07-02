using System.Collections.Generic;
using Api.Models;
using Data.DataService.Enums;
using MongoDB.Driver;

namespace Api.Services
{
    public class AuthorService
    {
        private readonly IMongoCollection<Author> _authors;

        public AuthorService(IMongoClient client, INewsStoreDatabaseSettings settings)
        {
            var database = client.GetDatabase(settings.DatabaseName);

            _authors = database.GetCollection<Author>(settings.AuthorCollectionName);
        }

        public List<Author> Get() =>
            _authors.Find(author => true).ToList();

        public Author Get(string id) =>
            _authors.Find(author => author.Id == id).FirstOrDefault();

        public List<Author> Search(string name) =>
            _authors.Find(Builders<Author>.Filter.Text(name)).ToList();

        public Author Create(Author author)
        {
            _authors.InsertOne(author);
            return author;
        }

        public Author Create(string authorName, Site site, string articleId)
        {
            if (string.IsNullOrEmpty(authorName))
            {
                return null;
            }
            var author = _authors.Find(Builders<Author>.Filter.Text(authorName)).FirstOrDefault();
            if (author == null)
            {
                var newAuthor = new Author(authorName, site, articleId);
                _authors.InsertOne(newAuthor);
                author = newAuthor;
            }
            else
            {
                author.Count++;
                author.Articles.Add(articleId);
                Update(author.Id, author);
            }

            return author;
        }

        public void Update(string id, Author authorIn) =>
            _authors.ReplaceOne(author => author.Id == id, authorIn);

        public void Remove(Author authorIn) =>
            _authors.DeleteOne(author => author.Id == authorIn.Id);
        public void Remove(string id)
        {
            var author = _authors.Find(author => author.Id == id).FirstOrDefault();
            author.Count--;
            if (author.Count <= 0)
            {
                _authors.DeleteOne(author => author.Id == id);
            }
        }
        
        public bool NameExists(string name)
        {
            return _authors.Find(a => a.Name == name).FirstOrDefault() != null;
        }
    }
}