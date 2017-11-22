using MongoDB.Driver;
using System;
using System.Linq;

namespace API.Repositories.Mongo
{
    public abstract class Base<T>
    {
        private Databases.Mongo Client { get; set; }

        private string CollectionName { get; set; }

        private string TypeName { get { return typeof(T).Name.Replace("-", "").Replace(".", "_").Replace("__", "_"); } }

        public IMongoCollection<T> Collection { get { return GetCollection(); } }

        public IQueryable<T> QueryableCollection { get { return Queryable(); } }

        public Base(Databases.Mongo client, string Name = "")
        {
            Client = client ?? throw new Exception("Injected Database mustn't be null");

            CollectionName = Name ?? TypeName;
        }

        private IMongoCollection<T> GetCollection()
        {
            return Client.Database.GetCollection<T>(CollectionName);
        }

        private IQueryable<T> Queryable()
        {
            return GetCollection().AsQueryable();
        }

        public abstract void Migration();

        public abstract bool Save(T Document);

        public abstract bool Delete(T Document);

        public abstract T GetOne(string Id);
    }
}