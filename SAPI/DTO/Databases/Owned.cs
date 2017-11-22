namespace DTO.Databases
{
    public class Owned
    {
        public string Owner { get; set; }

        public string Group { get; set; }

        public long CreatedAt { get; set; }

        public long ModifiedAt { get; set; }

        public Owned()
        {
            Owner = "NOONE";
            Group = "NONE";
            CreatedAt = Generator.Tick();
            ModifiedAt = Generator.Tick();
        }
    }
}