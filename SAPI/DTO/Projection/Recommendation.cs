using System.Collections.Generic;

namespace DTO.Projection
{
    public class Recommendation
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public long Price { get; set; }

        public HashSet<string> Images { get; set; }

        public Recommendation()
        {
            Id = Generator.Id();
            Name = "";
            Price = -1;
            Images = new HashSet<string>();
        }
    }
}