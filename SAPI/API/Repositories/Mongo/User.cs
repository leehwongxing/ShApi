using MongoDB.Bson;
using System;
using System.Linq;

namespace API.Repositories.Mongo
{
    public class User : Base<DTO.Databases.User>
    {
        public User(Databases.Mongo client) : base(client, "Users")
        {
            Migration();
        }

        public override bool Delete(DTO.Databases.User Document)
        {
            var Id = Document.Id;

            if (string.IsNullOrWhiteSpace(Id))
            {
                throw new Exception("Id of to be Deleted Document can't empty");
            }

            var Filter = new BsonDocument("_id", Document.Id);
            Collection.DeleteOne(Filter);
            return true;
        }

        public override DTO.Databases.User GetOne(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                throw new Exception("Id can't be null");
            }

            var Queried = QueryableCollection.Where(x => x.Id == Id);

            if (Queried.Count() != 1)
            {
                return null;
            }
            else
            {
                return Queried.First();
            }
        }

        public override void Migration()
        {
            var Count = QueryableCollection.LongCount();

            if (Count == 0)
            {
                var DefaultUser = new DTO.Databases.User()
                {
                    Email = "leehwongxing@yandex.ru",
                    Fullname = "leehwongxing"
                };

                DefaultUser.Roles.Add("Administrator");
                DefaultUser.Password = Configs.Hashing.Hash("Bo Trong Cung Duoc", DefaultUser.Id);

                Collection.InsertOne(DefaultUser);
            }
        }

        public override bool Save(DTO.Databases.User Document)
        {
            var Queried = QueryableCollection.Where(x => x.Id == Document.Id);

            if (Queried.Count() == 0)
            {
                Collection.InsertOne(Document);
                return true;
            }
            else
            {
                var Old = Queried.First();

                if (Old.ModifiedAt >= Document.ModifiedAt)
                {
                    return false;
                }

                var Filter = new BsonDocument("_id", Document.Id);
                Collection.ReplaceOne(Filter, Document);
                return true;
            }
        }
    }
}