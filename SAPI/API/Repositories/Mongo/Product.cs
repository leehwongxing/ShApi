using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

namespace API.Repositories.Mongo
{
    public class Product : Base<DTO.Databases.Product>
    {
        public Product(Databases.Mongo client) : base(client, "Products")
        {
        }

        protected override void MigrateData()
        {
            Collection.Indexes.CreateOne(Builders<DTO.Databases.Product>.IndexKeys.Text(x => x.Name));
            base.MigrateData();
        }

        public override bool Delete(DTO.Databases.Product Document)
        {
            if (string.IsNullOrWhiteSpace(Document.Id))
            {
                return false;
            }

            var Filter = new BsonDocument("_id", Document.Id);
            Collection.DeleteOne(Filter);
            return true;
        }

        public override DTO.Databases.Product GetOne(string Id)
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

        public override bool Save(DTO.Databases.Product Document)
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