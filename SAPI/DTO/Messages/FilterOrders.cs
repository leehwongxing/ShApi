namespace DTO.Messages
{
    public class FilterOrders
    {
        public string Stage { get; set; }

        public bool Queued { get; set; }

        public FilterOrders()
        {
            Stage = "";
            Queued = false; // true tức là stage được filter là của người đang lọc
        }
    }
}