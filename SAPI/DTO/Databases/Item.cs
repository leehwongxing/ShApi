using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DTO.Databases
{
    public class Item : Owned
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long Price { get; set; }

        public HashSet<string> Images { get; set; }

        public HashSet<string> Categories { get; set; }

        public HashSet<string> SubCategories { get; set; }

        public Dictionary<string, Promotion> Promotions { get; set; }

        public Item() : base()
        {
            Id = Generator.Id();
            Name = "";
            Description = "";
            Price = 0;
            Images = new HashSet<string>();
            Categories = new HashSet<string>();
            SubCategories = new HashSet<string>();
            Promotions = new Dictionary<string, Promotion>();
            Group = "ITEM";
        }
    }
}