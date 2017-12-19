using MongoDB.Bson;
using System.Linq;

namespace API.Repositories.Mongo
{
    public class SubCategory : Base<DTO.Databases.SubCategory>
    {
        public SubCategory(Databases.Mongo Client) : base(Client, "SubCategories")
        {
        }

        public override bool Delete(DTO.Databases.SubCategory Document)
        {
            if (string.IsNullOrWhiteSpace(Document.Id))
            {
                return false;
            }

            var Filter = new BsonDocument("_id", Document.Id);
            Collection.DeleteOne(Filter);
            return true;
        }

        public override DTO.Databases.SubCategory GetOne(string Id)
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
            else
            {
                return Query.First();
            }
        }

        public override bool Save(DTO.Databases.SubCategory Document)
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