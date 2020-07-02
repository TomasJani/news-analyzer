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

        public async Task Invoke()
        {
            await Load();
        }

        public async Task Load()
        {
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd");
            await Load(dateTime);
        }

        public async Task Load(string dateTime)
        {
            var rawArticles = await GetArticles(dateTime) ?? new List<RawArticle>();
            using var client = new HttpClient();
            foreach (var rawArticle in rawArticles)
            {
                var articleJson = rawArticle.ToString();
                var response = await client.PostAsync(
                    $"{_settings.ApiUrl}{_settings.ArticlesEndpoint}", 
                    new StringContent(articleJson, Encoding.UTF8, "application/json"));
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    _logger.Log(LogLevel.Warning, $"Article with title {rawArticle.Title} received wrong response code <{response.StatusCode}>");
                }
            }
        }
        
        public async Task InitialLoad(uint loadDays)
        {
            var dayToLoad = DateTime.Now;
            for (var i = 0; i < loadDays; ++i)
            {
                dayToLoad = dayToLoad.AddDays(-1);
                await Load(dayToLoad.ToString("yyyy-MM-dd"));
            }
        }

        private async Task<IEnumerable<RawArticle>> GetArticles(string dateTime)
        {
            var fileName = Path.Combine(_currentDirectory, _settings.DataFolder, $"{dateTime}.json");

            try
            {
                using var jsonFileReader = File.OpenText(fileName);
                return JsonConvert.DeserializeObject<RawArticle[]>(await jsonFileReader.ReadToEndAsync());
            }
            catch (Exception e) 
            {
                _logger.Log(LogLevel.Error, $"Error in reading OR deserializing file {fileName}/n{e}");
            }

            return null;
        }
    }
}