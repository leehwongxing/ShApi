namespace DTO.Databases
{
    public class Category : Owned
    {
        public string Id { get { return GetId(); } }

        public string Name { get; set; }

        public Category(string CatName = "") : base()
        {
            Name = CatName;
            Group = "CATEGORY";
        }

        public string GetId()
        {
            return Generator.StripAccents(Name).Trim().Replace(" ", "_").ToUpper();
        }
    }
}