using Microsoft.Extensions.Configuration;

namespace MLocker.Api
{
    public interface IConfig
    {
        string ConnectionString { get; }
        string StorageConnectionString { get; }
        string ApiKey { get; }
    }

    public class Config : IConfig
    {
        private readonly IConfiguration _configuration;

        public Config(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString => _configuration["ConnectionString"];
        public string StorageConnectionString => _configuration["StorageConnectionString"];
        public string ApiKey => _configuration["ApiKey"];
    }
}