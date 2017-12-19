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

            var Configs = ConfigurationOptions.Parse(Options.ConnectString);
            Configs.ClientName = Options.Username;
            Configs.AllowAdmin = false;
            Configs.ConnectTimeout = 10000;
            if (Options.Database >= 0)
            {
                Configs.DefaultDatabase = Options.Database;
            }
            if (!string.IsNullOrEmpty(Options.Password))
            {
                Configs.Password = Options.Password;
            }
            Connection = ConnectionMultiplexer.Connect(Configs);
        }

        private IDatabase GetDatabase()
        {
            return Connection.GetDatabase(Options.Database);
        }
    }
}