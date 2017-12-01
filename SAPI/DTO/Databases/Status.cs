namespace DTO.Databases
{
    public class Status
    {
        public long Received { get; set; }

        public long Prepared { get; set; }

        public long Delivering { get; set; }

        public long FulFilled { get; set; }

        public long Canceled { get; set; }

        public Status()
        {
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