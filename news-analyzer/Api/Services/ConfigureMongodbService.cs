using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Shared.Models;
using Tag = Shared.Models.Tag;

namespace Api.Services
{
    public class ConfigureMongodbService : IHostedService
    {
        private readonly IMongoClient _client;
        private readonly ILogger<ConfigureMongodbService> _logger;
        private readonly INewsStoreDatabaseSettings _settings;

        public ConfigureMongodbService(IMongoClient client, ILogger<ConfigureMongodbService> logger, INewsStoreDatabaseSettings settings)
            => (_client, _logger, _settings) = (client, logger, settings);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Deleting Db for debugging
            await _client.DropDatabaseAsync(_settings.DatabaseName, cancellationToken);
            var database = _client.GetDatabase(_settings.DatabaseName);

            var articles = database.GetCollection<Article>(_settings.ArticleCollectionName);
            var tags = database.GetCollection<Tag>(_settings.TagCollectionName);
            var authors = database.GetCollection<Author>(_settings.AuthorCollectionName);
            
            var articleKeys = Builders<Article>.IndexKeys.Combine(
                Builders<Article>.IndexKeys.Text(article => article.Title),
                Builders<Article>.IndexKeys.Text(article => article.Description),
                Builders<Article>.IndexKeys.Text(article => article.Content));
            var tagKeys = Builders<Tag>.IndexKeys.Text(tag => tag.Name);
            var authorKeys = Builders<Author>.IndexKeys.Combine(
                Builders<Author>.IndexKeys.Text(author => author.Name),
                Builders<Author>.IndexKeys.Text(author => author.Site));

            _logger.LogInformation("Creating MongoDb index on articles");
            await articles.Indexes.CreateOneAsync(new CreateIndexModel<Article>(articleKeys), cancellationToken: cancellationToken);
            
            _logger.LogInformation("Creating MongoDb index on tags");
            await tags.Indexes.CreateOneAsync(new CreateIndexModel<Tag>(tagKeys), cancellationToken: cancellationToken);
            
            _logger.LogInformation("Creating MongoDb index on authors");
            await authors.Indexes.CreateOneAsync(new CreateIndexModel<Author>(authorKeys), cancellationToken: cancellationToken);
        }


        public Task StopAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
