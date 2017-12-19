using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Messages
{
    public class Product
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public long Price { get; set; }

        public HashSet<string> Images { get; set; }

        public HashSet<string> Categories { get; set; }

        public HashSet<string> SubCategories { get; set; }

        public Product() : base()
        {
            Name = "";
            Description = "";
            Price = 0;
            Images = new HashSet<string>();
            Categories = new HashSet<string>();
            SubCategories = new HashSet<string>();
        }
    }
}