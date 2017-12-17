using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace API.Databases
{
    public class Mongo
    {
        private Configs.Mongo Options { get; set; }

        private MongoClient Client { get; set; }

        public IMongoDatabase Database { get { return GetDatabase(); } }

        public Mongo(IOptionsSnapshot<Configs.Mongo> options)
        {
            Options = options.Value;

            var MongoSettings = MongoClientSettings.FromUrl(new MongoUrl(Options.ConnectString));

            if (!string.IsNullOrEmpty(Options.Username) && !string.IsNullOrEmpty(Options.Password) && !string.IsNullOrEmpty(Options.Database))
            {
                MongoSettings.Credential = MongoCredential.CreateCredential(Options.Database, Options.Username, Options.Password);
            }
            Client = new MongoClient(MongoSettings);
        }

        private IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(Options.Database);
        }
    }
}