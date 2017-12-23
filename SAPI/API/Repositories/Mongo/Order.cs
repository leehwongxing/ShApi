using MongoDB.Bson;
using System.Linq;

namespace API.Repositories.Mongo
{
    public class Order : Base<DTO.Databases.Order>
    {
        public Order(Databases.Mongo client) : base(client, "Orders")
        {
        }

        public override bool Delete(DTO.Databases.Order Document)
        {
            if (string.IsNullOrWhiteSpace(Document.Id))
            {
                return false;
            }

            var Filter = new BsonDocument("_id", Document.Id);
            Collection.DeleteOne(Filter);
            return true;
        }

        public override DTO.Databases.Order GetOne(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                return null;
            }

            var Query = QueryableCollection.Where(x => x.Id == Id);
            if (Query.Count() == 0)
            {
                return null;
            }
            return Query.First();
        }

        public override bool Save(DTO.Databases.Order Document)
        {
            if (string.IsNullOrWhiteSpace(Document.Id) || string.IsNullOrWhiteSpace(Document.OrdererId))
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