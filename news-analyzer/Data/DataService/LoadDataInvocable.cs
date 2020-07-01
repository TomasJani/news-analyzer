using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Data.DataService
{
    public class LoadDataInvocable : IInvocable
    {
        private readonly string _currentDirectory = Directory.GetParent(Environment.CurrentDirectory)?.FullName;
        private readonly IDataSettings _settings;
        private readonly ILogger<LoadDataInvocable> _logger;
        
        public LoadDataInvocable(IDataSettings settings, ILogger<LoadDataInvocable> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public Task Invoke()
        {
            Load();
            return Task.CompletedTask;
        }

        public async void Load()
        {
            var rawArticles = GetArticles() ?? new List<RawArticle>();
            using var client = new HttpClient();
            foreach (var rawArticle in rawArticles)
            {
                var articleJson = rawArticle.ToString();
                // add try catch
                var response = await client.PostAsync(
                    $"{_settings.ApiUrl}{_settings.ArticlesEndpoint}", 
                    new StringContent(articleJson, Encoding.UTF8, "application/json"));
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    _logger.Log(LogLevel.Warning, $"Article with title {rawArticle.Title} received wrong response code <{response.StatusCode}>");
                }
            }
        }

        private IEnumerable<RawArticle> GetArticles()
        {
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd");
            var fileName = Path.Combine(_currentDirectory, _settings.DataFolder, $"{dateTime}.json");

            try
            {
                using var jsonFileReader = File.OpenText(fileName);
                return JsonConvert.DeserializeObject<RawArticle[]>(jsonFileReader.ReadToEnd());
            }
            catch (Exception e) 
            {
                _logger.Log(LogLevel.Error, $"Error in reading OR deserializing file {fileName}/n{e}");
            }

            return null;
        }
    }
}