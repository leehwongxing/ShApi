using MongoDB.Bson;
using System.Linq;

namespace API.Repositories.Mongo
{
    public class Resource : Base<DTO.Databases.Resource>
    {
        public Resource(Databases.Mongo Client) : base(Client, "Resources")
        {
        }

        public override bool Delete(DTO.Databases.Resource Document)
        {
            if (string.IsNullOrWhiteSpace(Document.Id))
            {
                return false;
            }

            var Filter = new BsonDocument("_id", Document.Id);
            Collection.DeleteOne(Filter);
            return true;
        }

        public override DTO.Databases.Resource GetOne(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                return null;
            }

            var Count = QueryableCollection.Where(x => x.Id == Id).Count();
            if (Count == 0)
            {
                return null;
            }
            else
            {
                return QueryableCollection.Where(x => x.Id == Id).First();
            }
        }

        public override bool Save(DTO.Databases.Resource Document)
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