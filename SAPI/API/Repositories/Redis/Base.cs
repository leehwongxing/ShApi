using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories.Redis
{
    public abstract class Base<T>
    {
        private string BucketName { get; set; }

        private Databases.Redis Client { get; set; }

        public string TypeName { get { return typeof(T).Name.Replace("-", "").Replace(".", "_").Replace("__", "_"); } }

        public Base(Databases.Redis client, string Name = "")
        {
            Client = client ?? throw new Exception("Injected RedisClient mustn't be null");

            BucketName = Name ?? TypeName;
        }

        public abstract void Save(T Document);

        public abstract void Delete(string Key);

        public abstract T GetOne(string Key);

        public abstract IEnumerable<T> GetList(string Key, int Position);
    }
}