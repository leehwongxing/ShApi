using MongoDB.Bson.Serialization.Attributes;

namespace DTO.Databases
{
    public class Permission : Owned
    {
        [BsonId]
        public string Id { get { return GetId(); } }

        public bool Owned { get; set; }

        public string Granted { get; set; }

        public string Description { get; set; }

        public Permission() : base()
        {
            Group = "PERMISSION";
            Owned = false;
            Granted = "NONE";
            Description = "EMPTY";
        }

        private string GetId()
        {
            return string.Join("__",
                Group,
                (Owned ? "TRUE" : "FALSE"),
                (Granted.Replace(" ", "_").Replace("-", "_").Replace("__", "_"))
                ).ToUpperInvariant();
        }
    }
}