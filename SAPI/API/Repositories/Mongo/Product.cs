using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace API.Repositories.Mongo
{
    public class Product : Base<DTO.Databases.Product>
    {
        public Product(Databases.Mongo client) : base(client, "Products")
        {
        }

        public IEnumerable<DTO.Databases.Product> SearchThru(DTO.Messages.SearchPage Search, bool TimeBased = true)
        {
            var Cached = Collection
                    .FindSync(Builders<DTO.Databases.Product>.Filter.Text(Search.SearchTerm))
                    .ToList();

            var Query = Cached.AsQueryable();
            if (Search.MaxPrice < long.MaxValue)
            {
                Query.Where(x => x.Price <= Search.MaxPrice);
            }
            if (Search.MinPrice > 0)
            {
                Query.Where(x => x.Price >= Search.MinPrice);
            }
            if (Search.Categories.Count > 0)
            {
                Query = Query.Where(x => Search.Categories.Intersect(x.Categories).Count() > 0);
            }
            if (Search.SubCategories.Count > 0)
            {
                Query = Query.Where(x => Search.SubCategories.Intersect(x.SubCategories).Count() > 0);
            }
            if (TimeBased)
            {
                Query.Where(x => x.Scored() > 0);
            }

            return Query;
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