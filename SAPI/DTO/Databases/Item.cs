using System.Collections.Generic;

namespace DTO.Databases
{
    public class Item : Owned
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long Price { get; set; }

        public HashSet<string> Images { get; set; }

        public HashSet<string> Categories { get; set; }

        public HashSet<string> SubCategories { get; set; }
    }
}