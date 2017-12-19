using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DTO.Databases
{
    public class Order : Owned
    {
        [BsonId]
        public string Id { get; set; }

        public string OrdererId { get; set; }

        public Address Delivery { get; set; }

        public Address Payment { get; set; }

        public string Note { get; set; }

        public Status OrderStatus { get; set; }

        public Dictionary<Product, int> Ordered { get; set; }

        public Order() : base()
        {
            Id = Generator.Id();
            OrdererId = "";
            Delivery = null;
            Payment = null;
            Note = "";
            OrderStatus = new Status();
            Ordered = new Dictionary<Product, int>();
            Group = "ORDER";
        }
    }
}