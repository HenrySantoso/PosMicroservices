namespace Play.Base.Service.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}
