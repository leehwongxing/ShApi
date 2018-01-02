using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DTO.Databases
{
    public class Token : Owned
    {
        [BsonId]
        public string TokenId { get; set; }

        public string UserId { get; set; }

        public Dictionary<string, int> Cart { get; set; }

        public Messages.TemporaryAddress PaymentAddress { get; set; }

        public Messages.TemporaryAddress DeliveryAddress { get; set; }

        public Token()
        {
            TokenId = "";
            UserId = "";
            Cart = new Dictionary<string, int>();
            PaymentAddress = null;
            DeliveryAddress = null;
            Group = "TOKEN";
        }
    }
}