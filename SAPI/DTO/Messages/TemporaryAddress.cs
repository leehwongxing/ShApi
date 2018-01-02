namespace DTO.Messages
{
    public class TemporaryAddress
    {
        public string Location { get; set; }

        public string Phone { get; set; }

        public string Recipent { get; set; }

        public string Note { get; set; }

        public TemporaryAddress()
        {
            Location = "";
            Phone = "";
            Recipent = "";
            Note = "";
        }
    }
}