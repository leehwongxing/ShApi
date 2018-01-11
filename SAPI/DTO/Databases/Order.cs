using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DTO.Databases
{
    public class Order : Owned
    {
        [BsonId]
        public string Id { get; set; }

        public string OrdererId { get; set; }

        public Messages.TemporaryAddress Delivery { get; set; }

        public Messages.TemporaryAddress Payment { get; set; }

        public string Note { get; set; }

        public Status OrderStatus { get; set; }

        public Dictionary<Projection.Recommendation, int> Ordered { get; set; }

        public Order() : base()
        {
            Id = Generator.Id();
            OrdererId = "";
            Delivery = null;
            Payment = null;
            Note = "";
            OrderStatus = new Status();
            Ordered = new Dictionary<Projection.Recommendation, int>();
            Group = "ORDER";
        }
    }
}