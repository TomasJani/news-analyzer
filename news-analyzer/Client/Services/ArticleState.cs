using System;
using System.Collections.Generic;
using System.Net.Http;
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
            Articles = JsonConvert.DeserializeObject<Article[]>(await response.Content.ReadAsStringAsync());
            IsLoaded = true;
            NotifyStateChanged();
        }
        
        // public async Task Search(SearchCriteria criteria)
        // {
        //     IsLoaded = false;
        //     
        //     Articles = await _httpClient.PostAsync("https://localhost:5001/api/articles/search", criteria);
        //     IsLoaded = true;
        // }
        //
        // public void AddToSavedSearches()
        // {
        //     
        // }
        
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}