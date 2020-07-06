using System.Collections.Generic;

namespace SharedModels.Models
{
    public class SavedSearchQuery
    {
        public SearchQuery SearchQuery { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}