using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharedModels.Models;

namespace Client.Services
{
    public class StorageState
    {
        private readonly IJSRuntime _jsRuntime;

        private readonly ArticleState _articleState;

        private bool _isDeserializing;

        public List<SavedSearchQuery> SearchQueries { get; set; } = new List<SavedSearchQuery>();

        public SearchQuery SearchQuery { get; set; } = new SearchQuery();

        public bool ShowAlert { get; set; }
        
        public StorageState(IJSRuntime jsRuntime, ArticleState articleState)
        {
            _jsRuntime = jsRuntime;
            _articleState = articleState;
        }
        
        public async Task DeleteFromLocal(SavedSearchQuery searchQuery)
        {
            SearchQueries.Remove(searchQuery);
        
            var savedJson = JsonConvert.SerializeObject(SearchQueries);
            await _jsRuntime.InvokeAsync<object>(
                "stateManager.save", "_searchQueriesList", savedJson);
        
            await LoadSavedSearches();
        }
        
        public async Task LoadSavedSearches()
        {
            var searchListJson = await _jsRuntime.InvokeAsync<string>("stateManager.load", "_searchQueriesList") ?? "";
            SearchQueries = JsonConvert.DeserializeObject<List<SavedSearchQuery>>(searchListJson) ?? new List<SavedSearchQuery>();
        }
        
        public async Task SaveToLocalStorage(string searchQueryName)
        {
            if (string.IsNullOrEmpty(searchQueryName))
                return;

            var searchListJson = await _jsRuntime.InvokeAsync<string>("stateManager.load", "_searchQueriesList") ?? "";
            var listStorage = JsonConvert.DeserializeObject<List<SavedSearchQuery>>(searchListJson) ?? new List<SavedSearchQuery>();
            var savedSearchQuery = new SavedSearchQuery
            {
                Name = searchQueryName, Articles = _articleState.Articles, SearchQuery = SearchQuery
            };


            listStorage.Add(savedSearchQuery);

            var savedJson = JsonConvert.SerializeObject(listStorage);
            await _jsRuntime.InvokeAsync<object>(
                "stateManager.save", "_searchQueriesList", savedJson);
            ShowAlert = true;
        }
        
        public async Task Initialize()
        {
            var searchQueryJson = await _jsRuntime.InvokeAsync<string>("stateManager.load", "_searchQuery");

            if (searchQueryJson != null)
            {
                var searchQueryStorage = JsonConvert.DeserializeObject<SearchQuery>(searchQueryJson);
                if (searchQueryStorage != null)
                {
                    if (SearchQuery == searchQueryStorage)
                        return;
                
                    _isDeserializing = true;
                    SearchQuery.Text = searchQueryStorage.Text;
                    SearchQuery.AuthorName = searchQueryStorage.AuthorName;
                    SearchQuery.TagName = searchQueryStorage.TagName;
                    SearchQuery.SearchHomeNews = searchQueryStorage.SearchHomeNews;
                    SearchQuery.SearchForeignNews = searchQueryStorage.SearchForeignNews;
                    SearchQuery.StartDate = searchQueryStorage.StartDate;
                    SearchQuery.EndDate = searchQueryStorage.EndDate;
                    _isDeserializing = false;
                }
            }
            SearchQuery.PropertyChanged += async (o, e) =>
            {
                if (_isDeserializing)
                {
                    return;
                }
                var searchQuerySave = JsonConvert.SerializeObject(SearchQuery);
                await _jsRuntime.InvokeAsync<object>(
                    "stateManager.save", "_searchQuery", searchQuerySave);
            };
        }
    }
}