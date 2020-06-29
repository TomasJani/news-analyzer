using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coravel.Invocable;
using Newtonsoft.Json;

namespace Data.DataService
{
    public class LoadDataInvocable : IInvocable
    {
        private readonly string _currentDirectory = Directory.GetParent(Environment.CurrentDirectory)?.FullName;
        private readonly IDataSettings _settings;

        public LoadDataInvocable(IDataSettings settings)
        {
            _settings = settings;
        }

        public Task Invoke()
        {
            Load();
            return Task.CompletedTask;
        }

        public async void Load()
        {
            var rawArticles = GetArticles();
            using var client = new HttpClient();
            foreach (var rawArticle in rawArticles)
            {
                var articleJson = rawArticle.ToString();
                // add try catch
                var response = await client.PostAsync(
                    "http://localhost:5000/api/articles/raw", 
                    new StringContent(articleJson, Encoding.UTF8, "application/json"));
            }
        }
        
        public IEnumerable<RawArticle> GetArticles()
        {
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd");
            var fileName = Path.Combine(_currentDirectory, _settings.DataFolder, $"{dateTime}.json");
            using var jsonFileReader = File.OpenText(fileName);
            return JsonConvert.DeserializeObject<RawArticle[]>(jsonFileReader.ReadToEnd());
        }
    }
}