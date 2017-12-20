using System.Collections.Generic;

namespace DTO.Messages
{
    public class SearchPage
    {
        public string SearchTerm { get; set; }

        public HashSet<string> Categories { get; set; }

        public HashSet<string> SubCategories { get; set; }

        public long MinPrice { get; set; }

        public long MaxPrice { get; set; }

        public int Page { get; set; }

        public int PageLimit { get; set; }

        public SearchPage()
        {
            SearchTerm = "";
            Categories = new HashSet<string>();
            SubCategories = new HashSet<string>();
            MinPrice = 1;
            MaxPrice = long.MaxValue;
            Page = 0;
            PageLimit = 24;
        }
    }
}