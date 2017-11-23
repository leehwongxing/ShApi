namespace DTO.Databases
{
    public class SubCategory : Category
    {
        public string ParentId { get; set; }

        public SubCategory() : base()
        {
            ParentId = "";
            Group = "SUBCATEGORY";
        }
    }
}