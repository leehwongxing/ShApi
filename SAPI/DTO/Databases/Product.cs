using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DTO.Databases
{
    public class Product : Owned
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

        public Product() : base()
        {
            Id = Generator.Id();
            Name = "";
            Description = "";
            Price = -1;
            Images = new HashSet<string>();
            Categories = new HashSet<string>();
            SubCategories = new HashSet<string>();
            Promotions = new Dictionary<string, Promotion>();
            Group = "ITEM";
        }

        public float Scored()
        {
            var Now = DateTime.UtcNow;
            var ActivePromo = Promotions.Values.Where(x => (
                        x.StartDate >= Now &&
                        x.EndDate <= Now &&
                        x.DaysOfWeek.Contains((int)Now.DayOfWeek) &&
                        x.StartTime <= Now.TimeOfDay &&
                        x.EndTime > Now.TimeOfDay));

            if (ActivePromo.Count() > 0)
            {
                return ActivePromo.Sum(x => x.Scored());
            }
            else
            {
                return 1; // hiện
            }
        }
    }
}