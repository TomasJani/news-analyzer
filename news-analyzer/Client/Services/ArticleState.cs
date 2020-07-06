using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Models;

namespace Client.Services
{
    public class ArticleState
    {
        public ICollection<Article> Articles { get; private set; } = new List<Article>();
        public bool IsLoaded { get; private set; } = false;

        private readonly HttpClient _httpClient;

        public ArticleState(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public event Action OnChange;

        public async Task Load()
        {
            IsLoaded = false;
            NotifyStateChanged();
            
            var response = await _httpClient.GetAsync("https://localhost:5001/api/articles");
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

            var jsonquery = JsonConvert.SerializeObject(searchQuery);
            Console.WriteLine(jsonquery);

            var response = await _httpClient.PostAsJsonAsync("https://localhost:5001/api/articles/search", searchQuery);
            if (response.IsSuccessStatusCode)
            {
                Articles = JsonConvert.DeserializeObject<Article[]>(await response.Content.ReadAsStringAsync());
            }
            IsLoaded = true;
            NotifyStateChanged();
        }
        
        public void AddToSavedSearches()
        {
            
        }
        
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}