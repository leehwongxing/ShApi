namespace DTO.Projection
{
    public class SubCategory : Category
    {
        public string Parent { get; set; }

        public SubCategory() : base()
        {
            Parent = "";
        }
    }
}