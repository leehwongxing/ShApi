using MongoDB.Bson;
using System.Linq;

namespace API.Repositories.Mongo
{
    public class Recommendation : Base<DTO.Databases.Recommendation>
    {
        public Recommendation(Databases.Mongo client) : base(client, "Recommendations")
        {
        }

        public override bool Delete(DTO.Databases.Recommendation Document)
        {
            if (string.IsNullOrWhiteSpace(Document.Id))
            {
                return false;
            }

            var Filter = new BsonDocument("_id", Document.Id);
            Collection.DeleteOne(Filter);
            return true;
        }

        public override DTO.Databases.Recommendation GetOne(string Id)
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

        public override bool Save(DTO.Databases.Recommendation Document)
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
            Collection.ReplaceOne(Filter, Document);
            return true;
        }
    }
}