using MongoDB.Bson;
using System;
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
                throw new Exception("Token's ID can't be empty");
            }

            var Filter = new BsonDocument("_id", Id);
            Collection.DeleteOne(Filter);
            return true;
        }

        public override DTO.Databases.Token GetOne(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                throw new Exception("Token's Id can't be empty");
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

        public override void Migration()
        {
            return;
        }

        public override bool Save(DTO.Databases.Token Document)
        {
            var Query = QueryableCollection.Where(x => x.TokenId == Document.TokenId);

            if (Query.Count() == 0)
            {
                Collection.InsertOne(Document);
            }
            else
            {
                var Old = Query.First();

                if (Old.ModifiedAt >= Document.ModifiedAt)
                {
                    return false;
                }

                var Filter = new BsonDocument("_id", Document.TokenId);
                Collection.ReplaceOne(Filter, Document);
            }
            return true;
        }
    }
}