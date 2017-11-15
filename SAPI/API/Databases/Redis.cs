using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace API.Databases
{
    public class Redis
    {
        private Configs.Redis Options { get; set; }

        private ConnectionMultiplexer Connection { get; set; }

        public IDatabase Database { get { return GetDatabase(); } }

        public int TTL { get { return Options.TTL; } }

        public Redis(IOptionsSnapshot<Configs.Redis> options)
        {
            Options = options.Value;

            var Configuration = ConfigurationOptions.Parse(Options.ConnectString);

            if (!string.IsNullOrEmpty(Options.Username) && !string.IsNullOrEmpty(Options.Password) && !(Options.Database >= 0))
            {
                Configuration.DefaultDatabase = Options.Database;
                Configuration.ClientName = Options.Username;
                Configuration.Password = Options.Password;
            }
            Connection = ConnectionMultiplexer.Connect(Configuration);
        }

        private IDatabase GetDatabase()
        {
            return Connection.GetDatabase(Options.Database);
        }
    }
}