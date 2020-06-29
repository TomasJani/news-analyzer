namespace Api.Models
{
    public class NewsStoreDatabaseSettings : INewsStoreDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string AuthorCollectionName { get; set; }
        public string ArticleCollectionName { get; set; }
        public string TagCollectionName { get; set; }
    }

    public interface INewsStoreDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string AuthorCollectionName { get; set; }
        public string ArticleCollectionName { get; set; }
        public string TagCollectionName { get; set; }
    }
}