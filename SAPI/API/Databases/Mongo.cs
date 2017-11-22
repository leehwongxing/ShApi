using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;

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
                var Credentials = new HashSet<MongoCredential>
                {
                    MongoCredential.CreateCredential(Options.Database, Options.Username, Options.Password)
                };

                MongoSettings.Credentials = Credentials;
            }
            Client = new MongoClient(MongoSettings);
        }

        private IMongoDatabase GetDatabase()
        {
            return Client.GetDatabase(Options.Database);
        }
    }
}