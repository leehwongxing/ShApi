namespace DTO.Databases
{
    public class Status
    {
        public Flag Received { get; set; }

        public Flag Prepared { get; set; }

        public Flag Delivering { get; set; }

        public Flag FulFilled { get; set; }

        public Flag Canceled { get; set; }

        public Status()
        {
            Received = null;
            Prepared = null;
            Delivering = null;
            FulFilled = null;
            Canceled = null;
        }

        public bool IsCanceled()
        {
            return Canceled != null || Canceled.Who != "" ? true : false;
        }

        public bool ProceedStage(string Stage = "", Flag Setter = null)
        {
            if (string.IsNullOrWhiteSpace(Stage) || Setter == null || string.IsNullOrWhiteSpace(Setter.Who) || string.IsNullOrWhiteSpace(Setter.Why))
                return false;

            switch (Stage.ToLower())
            {
                case "received": // cần xử lí
                    Received = Setter;
                    break;

                case "prepared": // cần chuẩn bị
                    if (Received == null)
                        return false;
                    Prepared = Setter;
                    break;

                case "delivering": // cần vận chuyển
                    if (Prepared == null)
                        return false;
                    Delivering = Setter;
                    break;

                case "fulfilled": // cần giao hàng
                    if (Delivering == null)
                        return false;
                    FulFilled = Setter;
                    break;

                case "canceled": // đã hủy
                    if (Prepared != null)
                        return false;
                    Canceled = Setter;
                    break;

                default:
                    return false;
            }
            return true;
        }
    }
}