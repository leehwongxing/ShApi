namespace DTO.Databases
{
    public class Category : Owned
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Category() : base()
        {
            Id = "";
            Name = "";
            Group = "CATEGORY";
        }
    }
}