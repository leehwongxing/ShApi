namespace DTO.Messages
{
    public class FilterPromotion
    {
        public string Name { get; set; }

        public string PlaceHolder { get; set; }

        public FilterPromotion()
        {
            Name = "";
            PlaceHolder = "";
        }
    }
}