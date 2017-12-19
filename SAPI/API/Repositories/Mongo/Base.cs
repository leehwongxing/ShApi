using Jil;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace API.Repositories.Mongo
{
    public abstract class Base<T>
    {
        private Databases.Mongo Client { get; set; }

        private string CollectionName { get; set; }

        private string MigrationFolder { get; set; }

        private string TypeName { get { return typeof(T).Name.Replace("-", "").Replace(".", "_").Replace("__", "_"); } }

        public IMongoCollection<T> Collection { get { return GetCollection(); } }

        public IQueryable<T> QueryableCollection { get { return Queryable(); } }

        public Base(Databases.Mongo client, string Name = "")
        {
            Client = client ?? throw new Exception("Injected Database mustn't be null");
            MigrationFolder = "UploadedData";
            CollectionName = Name ?? TypeName;
            MigrateData();
        }

        private IMongoCollection<T> GetCollection()
        {
            return Client.Database.GetCollection<T>(CollectionName);
        }

        private IQueryable<T> Queryable()
        {
            return GetCollection().AsQueryable();
        }

        public void MigrateData()
        {
            var FilePath = Path.Combine(MigrationFolder, CollectionName);
            if (!File.Exists(FilePath))
            {
                return;
            }

            try
            {
                HashSet<T> Data = null;
                using (var Stream = new StreamReader(File.OpenRead(FilePath), Encoding.UTF8, true, 10240))
                {
                    Data = JSON.Deserialize<HashSet<T>>(Stream.ReadToEnd());
                }
                if (QueryableCollection.Count() == 0)
                {
                    Collection.InsertMany(Data);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public abstract bool Save(T Document);

        public abstract bool Delete(T Document);

        public abstract T GetOne(string Id);
    }
}