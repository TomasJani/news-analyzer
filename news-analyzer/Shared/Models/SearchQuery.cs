using System;
using System.Timers;
using Newtonsoft.Json;

namespace Shared.Models
{
    public class SearchQuery
    {
        [JsonIgnore]
        public Timer DebounceTimer = new Timer
        {
            Interval = 300,
            AutoReset = false
        };

        private string _text;
        private string _tagName;
        private string _authorName;
        private bool _searchHomeNews = true;
        private bool _searchForeignNews;
        private DateTimeOffset? _startDate = DateTime.Today.AddMonths(-1);
        private DateTimeOffset? _endDate = DateTime.Today.AddDays(1).AddTicks(-1);

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                DebounceTimer?.Stop();
                DebounceTimer?.Start();
            }
        }

        public string TagName
        {
            get => _tagName;
            set
            {
                _tagName = value;
                DebounceTimer?.Stop();
                DebounceTimer?.Start();
            }
        }

        public string AuthorName
        {
            get => _authorName;
            set
            {
                _authorName = value;
                DebounceTimer?.Stop();
                DebounceTimer?.Start();
            }
        }

        public bool SearchHomeNews
        {
            get => _searchHomeNews;
            set
            {
                _searchHomeNews = value;
                DebounceTimer?.Stop();
                DebounceTimer?.Start();
            }
        }

        public bool SearchForeignNews
        {
            get => _searchForeignNews;
            set
            {
                _searchForeignNews = value;
                DebounceTimer?.Stop();
                DebounceTimer?.Start();
            }
        }

        public DateTimeOffset? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                DebounceTimer?.Stop();
                DebounceTimer?.Start();
            }
        }

        public DateTimeOffset? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                DebounceTimer?.Stop();
                DebounceTimer?.Start();
            }
        }
    }
}
