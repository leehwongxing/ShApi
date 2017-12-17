using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Messages
{
    public class RegisteringUser
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Fullname { get; set; }

        public string Birthday { get; set; }

        public int Gender { get; set; }

        public RegisteringUser()
        {
            Email = "";
            Password = "";
            Fullname = "";
            Birthday = "";
            Gender = -1;
        }
    }
}