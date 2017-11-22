namespace DTO.Databases
{
    public class Address : Owned
    {
        public string Id { get; set; }

        public string Location { get; set; }

        public string Phone { get; set; }

        public string Recipent { get; set; }

        public string Type { get; set; }

        public string Note { get; set; }

        public bool InUse { get; set; }

        public Address() : base()
        {
            Id = Generator.Id();
            Location = "";
            Group = "ADDRESS";
            Phone = "";
            Recipent = "";
            Type = "";
            Note = "";
            InUse = true;
        }
    }
}