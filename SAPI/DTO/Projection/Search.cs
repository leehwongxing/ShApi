using System.Collections.Generic;

namespace DTO.Projection
{
    public class Search
    {
        public string SearchTerm { get; set; }

        public HashSet<string> Categories { get; set; }

        public HashSet<string> SubCategories { get; set; }

        public Search()
        {
            SearchTerm = "";
            Categories = new HashSet<string>();
            SubCategories = new HashSet<string>();
        }
    }
}