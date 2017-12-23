namespace DTO.Messages
{
    public class AddToCart
    {
        public string ProductId { get; set; }

        public int Quantity { get; set; }

        public AddToCart()
        {
            ProductId = "";
            Quantity = 1;
        }
    }
}