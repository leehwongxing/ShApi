using Jil;
using System;
using System.Collections.Generic;

namespace API.Repositories.Redis
{
    public abstract class Base<T>
    {
        private string BucketName { get; set; }

        private int TTL { get { return Client.TTL; } }

        private Databases.Redis Client { get; set; }

        protected string TypeName { get { return typeof(T).Name; } }

        public Base(Databases.Redis client, string Name = "")
        {
            Client = client ?? throw new Exception("Injected RedisClient mustn't be null");
            BucketName = DTO.Generator.StripAccents(Name ?? TypeName).Replace(" ", "_");
        }

        public string BucketKey(Object Key)
        {
            return string.Join(":___", BucketName, JSON.Serialize(Key, Options.ISO8601IncludeInherited));
        }

        public bool Save(T Document, Object Id)
        {
            return Client.Database.StringSet(BucketKey(Id), JSON.Serialize(Document, Options.ISO8601IncludeInherited), TimeSpan.FromSeconds(TTL));
        }

        public bool SaveList(IEnumerable<T> Documents, Object Id)
        {
            return Client.Database.StringSet(BucketKey(Id), JSON.Serialize(Documents, Options.ISO8601IncludeInherited), TimeSpan.FromSeconds(TTL));
        }

        public bool Delete(Object Id)
        {
            return Client.Database.KeyDelete(BucketKey(Id));
        }

        public T Get(Object Id)
        {
            var Key = BucketKey(Id);
            if (!Client.Database.KeyExists(Key))
            {
                return default(T);
            }

            try
            {
                var Data = Client.Database.StringGet(Key);
                return JSON.Deserialize<T>(Data);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public IEnumerable<T> GetList(Object Id)
        {
            var Key = BucketKey(Id);
            if (!Client.Database.KeyExists(Key))
            {
                return null;
            }

            try
            {
                var Data = Client.Database.StringGet(Key);
                return JSON.Deserialize<IEnumerable<T>>(Data);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}