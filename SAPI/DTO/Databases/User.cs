using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Databases
{
    public class User
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Fullname { get; set; }

        public HashSet<string> Roles { get; set; }

        public HashSet<Address> Addresses { get; set; }

        public string PaymentAddress { get; set; }

        public string DeliveryAddress { get; set; }

        public string Birthday { get; set; }

        public string Gender { get; set; }

        public User()
        {
            Id = Generator.Id();
            Email = "";
            Password = "";
            Fullname = "";
            Roles = new HashSet<string>();
            Addresses = new HashSet<Address>();
            PaymentAddress = "";
            DeliveryAddress = "";
            Birthday = "";
            Gender = "";
        }
    }
}