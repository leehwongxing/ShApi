using MongoDB.Driver;
using System;

namespace API.Repositories.Mongo
{
    public abstract class Base<T>
    {
        private Databases.Mongo Client { get; set; }

        private string CollectionName { get; set; }

        private string TypeName { get { return typeof(T).Name.Replace("-", "").Replace(".", "_").Replace("__", "_"); } }

        protected IMongoCollection<T> Collection { get { return GetCollection(); } }

        public Base(Databases.Mongo client, string Name = "")
        {
            Client = client ?? throw new Exception("Injected Database mustn't be null");

            CollectionName = Name ?? TypeName;
        }

        private IMongoCollection<T> GetCollection()
        {
            return Client.Database.GetCollection<T>(CollectionName);
        }

        public abstract void Save(T Document);

        public abstract void Delete(T Document);

        public abstract T GetOne(string Id);
    }
}