using System.Collections.Generic;

namespace DTO.Databases
{
    public class Recommended : Owned
    {
        public string Id { get; set; }

        public HashSet<string> List { get; set; }

        public string Reason { get; set; }

        public Recommended() : base()
        {
            Id = "";
            List = new HashSet<string>();
            Reason = "";
            Group = "RECOMMENDED";
        }
    }
}