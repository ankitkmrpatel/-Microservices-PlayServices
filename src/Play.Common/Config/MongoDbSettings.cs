namespace Play.Common.Config;

public class MongoDbSettings
{
    public string Host { get; init; }
    public int Port { get; init; }
    public string DatabaseName { get; init; }

    public string ConnectionString => $"mongodb://{Host}:{Port}";
}
