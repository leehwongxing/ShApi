using MongoDB.Bson;
using System.Linq;

namespace API.Repositories.Mongo
{
    public class Token : Base<DTO.Databases.Token>
    {
        public Token(Databases.Mongo Client) : base(Client, "Tokens")
        {
        }

        public override bool Delete(DTO.Databases.Token Document)
        {
            var Id = Document.TokenId;

            if (string.IsNullOrWhiteSpace(Id))
            {
                return false;
            }

            var Filter = new BsonDocument("_id", Id);
            Collection.DeleteOne(Filter);
            return true;
        }

        public override DTO.Databases.Token GetOne(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                return null;
            }

            var Query = QueryableCollection.Where(x => x.TokenId == Id);

            if (Query.Count() == 0)
            {
                return null;
            }
            else
            {
                return QueryableCollection.Where(x => x.TokenId == Id).First();
            }
        }

        public override bool Save(DTO.Databases.Token Document)
        {
            if (string.IsNullOrWhiteSpace(Document.TokenId))
            {
                return false;
            }
            var Query = QueryableCollection.Where(x => x.TokenId == Document.TokenId);

            if (Query.Count() > 0)
            {
                var Old = Query.First();
                if (Document.ModifiedAt < Old.ModifiedAt)
                {
                    return false;
                }
            }

            var Filter = new BsonDocument("_id", Document.TokenId);
            Collection.ReplaceOne(Filter, Document, new MongoDB.Driver.UpdateOptions { IsUpsert = true });
            return true;
        }
    }
}