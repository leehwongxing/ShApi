using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DTO.Databases
{
    public class Recommended : Owned
    {
        [BsonId]
        public string Id { get; set; }

        public HashSet<string> List { get; set; }

        public string Reason { get; set; }

        public Recommended() : base()
        {
            Id = Generator.Id();
            List = new HashSet<string>();
            Reason = "";
            Group = "RECOMMENDED";
        }
    }
}