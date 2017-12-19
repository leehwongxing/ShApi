using MongoDB.Bson;
using System.Linq;

namespace API.Repositories.Mongo
{
    public class User : Base<DTO.Databases.User>
    {
        public User(Databases.Mongo client) : base(client, "Users")
        {
        }

        public override bool Delete(DTO.Databases.User Document)
        {
            if (string.IsNullOrWhiteSpace(Document.Id))
            {
                return false;
            }

            var Filter = new BsonDocument("_id", Document.Id);
            Collection.DeleteOne(Filter);
            return true;
        }

        public override DTO.Databases.User GetOne(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                return null;
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

        public override bool Save(DTO.Databases.User Document)
        {
            if (string.IsNullOrWhiteSpace(Document.Id))
            {
                return false;
            }
            var Query = QueryableCollection.Where(x => x.Id == Document.Id);
            if (Query.Count() > 0)
            {
                var Old = Query.First();
                if (Document.ModifiedAt < Old.ModifiedAt)
                {
                    return false;
                }
            }
            var Filter = new BsonDocument("_id", Document.Id);
            Collection.ReplaceOne(Filter, Document, new MongoDB.Driver.UpdateOptions { IsUpsert = true });
            return true;
        }
    }
}