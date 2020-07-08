using System.Collections.Generic;

namespace SharedModels.Models
{
    public class SavedSearchQuery
    {
        public string Name { get; set; }
        public SearchQuery SearchQuery { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}