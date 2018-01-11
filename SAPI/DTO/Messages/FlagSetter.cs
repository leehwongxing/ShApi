using System.Collections.Generic;

namespace DTO.Messages
{
    public class FlagSetter
    {
        public string Stage { get; set; }

        public string Reason { get; set; }

        public HashSet<string> Orders { get; set; }

        public FlagSetter()
        {
            Stage = "";
            Reason = "";
            Orders = new HashSet<string>();
        }
    }
}