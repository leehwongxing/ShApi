using System.Collections.Generic;

namespace DTO.Databases
{
    public class Status
    {
        public string[] Allowed { get; set; }

        public HashSet<Flag> Flags { get; set; }

        public long Received { get; set; }

        public long Prepared { get; set; }

        public long Delivering { get; set; }

        public long FulFilled { get; set; }

        public long Canceled { get; set; }

        public Status()
        {
            Allowed = new string[] { "Received", "Prepared", "Delivering", "FulFilled", "Canceled" };
            Flags = new HashSet<Flag>();
            Received = Generator.Tick();
            Prepared = -1;
            Delivering = -1;
            FulFilled = -1;
            Canceled = -1;
        }

        public bool IsCanceled()
        {
            return Canceled > 0 ? true : false;
        }
    }
}