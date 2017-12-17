using MongoDB.Bson;
using System;
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
                throw new Exception("Resource's ID can't be empty");
            }

            var Filter = new BsonDocument("_id", Document.Id);
            Collection.DeleteOne(Filter);
            return true;
        }

        public override DTO.Databases.Resource GetOne(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                throw new Exception("Resource's ID can't be empty");
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

        public override void Migration()
        {
            return;
        }

        public override bool Save(DTO.Databases.Resource Document)
        {
            if (string.IsNullOrWhiteSpace(Document.Id))
            {
                throw new Exception("Resource's ID can't be empty");
            }

            var Count = QueryableCollection.Where(x => x.Id == Document.Id).Count();
            if (Count == 0)
            {
                Collection.InsertOne(Document);
                return true;
            }
            else
            {
                var Filter = new BsonDocument("_id", Document.Id);
                Collection.ReplaceOne(Filter, Document);
                return true;
            }
        }
    }
}