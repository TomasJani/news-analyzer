namespace Data
{
    public class DataSettings : IDataSettings
    {
        public string ApiUrl { get; set; }
        public string ArticlesEndpoint { get; set; }
        public string DataFolder { get; set; }
        public int LoadDataHour { get; set; }
        public uint InitialDaysLoad { get; set; }
    }

    public interface IDataSettings
    {
        public string ApiUrl { get; set; }
        public string ArticlesEndpoint { get; set; }
        public string DataFolder { get; set; }
        public int LoadDataHour { get; set; }
        public uint InitialDaysLoad { get; set; }
    }
}