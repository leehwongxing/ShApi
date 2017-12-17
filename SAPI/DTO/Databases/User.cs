using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DTO.Databases
{
    public class User : Owned
    {
        [BsonId]
        public string Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Fullname { get; set; }

        public HashSet<string> Roles { get; set; }

        public HashSet<Address> Addresses { get; set; }

        public string PaymentAddress { get; set; }

        public string DeliveryAddress { get; set; }

        public string Birthday { get; set; }

        public int Gender { get; set; } // 0 là Nam, 1 là Nữ

        public User() : base()
        {
            Id = Generator.Id();
            Group = "USER";
            Owner = Id;
            Email = "";
            Password = "";
            Fullname = "";
            Roles = new HashSet<string>();
            Addresses = new HashSet<Address>();
            PaymentAddress = "";
            DeliveryAddress = "";
            Birthday = "";
            Gender = 0;

            Roles.Add("Default");
        }
    }
}