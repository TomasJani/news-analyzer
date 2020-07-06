using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SharedModels.Enums;
using SharedModels.Models;

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

        public async Task<List<Author>> Get()
        {
            var authors = await _authors.FindAsync(author => true);
            return await authors.ToListAsync();
        }

        public async Task<Author> Get(string id)
        {
            var authors = await _authors.FindAsync(author => author.Id == id);
            return await authors.FirstOrDefaultAsync();
        }

        public async Task<List<Author>> Search(string name)
        {
            var authors = await _authors.FindAsync(Builders<Author>.Filter.Text(name));
            return await authors.ToListAsync();
        }
        
        public async Task<List<Article>> FilterByAuthor(List<Article> articles, string authorName)
        {
            if (string.IsNullOrEmpty(authorName)) 
                return articles;
            
            var authorSearch = await Search(authorName);
            var authors = authorSearch.Select(author => author.Id).ToList();
            return articles.Where(article => authors.Contains(article.AuthorId)).ToList();
        }

        public async Task<Author> Create(Author author)
        {
            await _authors.InsertOneAsync(author);
            return author;
        }

        public async Task<Author> Create(string authorName, Site site, string articleId)
        {
            if (string.IsNullOrEmpty(authorName)) 
                return null;

            var findAuthor = await _authors.FindAsync(Builders<Author>.Filter.Text(authorName));
            var author = await findAuthor.FirstOrDefaultAsync();
            if (author == null)
            {
                var newAuthor = new Author(authorName, site, articleId);
                await _authors.InsertOneAsync(newAuthor);
                author = newAuthor;
            }
            else
            {
                author.Count++;
                author.Articles.Add(articleId);
                await Update(author.Id, author);
            }

            return author;
        }

        public async Task Update(string id, Author authorIn) =>
            await _authors.ReplaceOneAsync(author => author.Id == id, authorIn);

        public async Task Remove(Author authorIn) =>
            await _authors.DeleteOneAsync(author => author.Id == authorIn.Id);

        public async Task Remove(string id)
        {
            var findAuthor = await _authors.FindAsync(a => a.Id == id);
            var author =  await findAuthor.FirstOrDefaultAsync();
            author.Count--;
            if (author.Count <= 0) 
                await _authors.DeleteOneAsync(a => a.Id == id);
        }

        public async Task<bool> NameExists(string name)
        {
            var findAuthor = await _authors.FindAsync(a => a.Name == name);
            return await findAuthor.FirstOrDefaultAsync() != null;
        }
    }
}