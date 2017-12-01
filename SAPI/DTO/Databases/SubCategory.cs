namespace DTO.Databases
{
    public class SubCategory : Category
    {
        public string ParentId { get; set; }

        public SubCategory(string CatName = "", string Parent = "") : base(CatName)
        {
            ParentId = Parent;
            Group = "SUBCATEGORY";
        }
    }
}