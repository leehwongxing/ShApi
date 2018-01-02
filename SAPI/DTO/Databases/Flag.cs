namespace DTO.Databases
{
    public class Flag
    {
        public string Who { get; set; }

        public long When { get; set; }

        public string What { get; set; }

        public string Why { get; set; }

        public Flag()
        {
            Who = "";
            When = -1;
            What = "";
            Why = "";
        }
    }
}