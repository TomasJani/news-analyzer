namespace Client
{
    public class ClientSettings : IClientSettings
    {
        public string ApiUrl { get; set; }
    }

    public interface IClientSettings
    {
        public string ApiUrl { get; set; }
    }
}