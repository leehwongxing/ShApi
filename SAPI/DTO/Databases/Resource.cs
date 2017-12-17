using MongoDB.Bson.Serialization.Attributes;

namespace DTO.Databases
{
    public class Resource : Owned
    {
        [BsonId]
        public string Id { get; set; }

        public long Length { get; set; }

        public string FileName { get; set; }

        public string MiMe { get; set; }

        public Resource() : base()
        {
            Id = Generator.Id();
            FileName = "";
            Length = 0;
            MiMe = "";
            Group = "RESOURCE";
        }
    }
}