using MongoDB.Bson.Serialization.Attributes;

namespace DTO.Databases
{
    public class Resource : Owned
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string Note { get; set; }

        public string Authorize { get; set; }

        public Resource() : base()
        {
            Id = Generator.Id();
            Name = "";
            Location = "";
            Note = "";
            Authorize = "";
            Group = "RESOURCE";
        }
    }
}