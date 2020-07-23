using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SharedModels.Models;

namespace Client.Services
{
    public class ArticleState
    {
        public ICollection<Article> Articles { get; private set; } = new List<Article>();
        public bool IsLoaded { get; private set; }

        private readonly HttpClient _httpClient;

        private readonly string _serverUrl;

        public ArticleState(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _serverUrl = configuration["ApiUrl"];
        }

        public event Action OnChange;

        public async Task Load()
        {
            IsLoaded = false;
            NotifyStateChanged();

            var response = await _httpClient.GetAsync($"{_serverUrl}articles");
            if (response.IsSuccessStatusCode)
            {
                Articles = JsonConvert.DeserializeObject<Article[]>(await response.Content.ReadAsStringAsync());
            }

            IsLoaded = true;
            NotifyStateChanged();
        }

        public async Task Search(SearchQuery searchQuery)
        {
            IsLoaded = false;
            NotifyStateChanged();

            var response = await _httpClient.PostAsJsonAsync($"{_serverUrl}articles/search", searchQuery);
            if (response.IsSuccessStatusCode)
            {
                Articles = JsonConvert.DeserializeObject<Article[]>(await response.Content.ReadAsStringAsync());
            }

            IsLoaded = true;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}