namespace DTO.Messages
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