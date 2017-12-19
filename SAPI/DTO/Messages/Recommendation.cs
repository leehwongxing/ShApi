using System.Collections.Generic;

namespace DTO.Messages
{
    public class Recommendation
    {
        public HashSet<string> List { get; set; }

        public string Reason { get; set; }

        public Recommendation() : base()
        {
            List = new HashSet<string>();
            Reason = "";
        }
    }
}