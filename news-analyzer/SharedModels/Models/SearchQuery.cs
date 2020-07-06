using System;
using System.ComponentModel;
using System.Timers;
using Newtonsoft.Json;

namespace SharedModels.Models
{
    public class SearchQuery : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagName)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AuthorName)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchHomeNews)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchForeignNews)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartDate)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndDate)));
                DebounceTimer?.Stop();
                DebounceTimer?.Start();
                
            }
        }
    }
}
