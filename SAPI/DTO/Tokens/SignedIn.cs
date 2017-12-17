using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Tokens
{
    public class SignedIn
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public DateTime SignedAt { get; set; }

        public string TokenType { get; set; }

        public string Token { get; set; }

        public SignedIn()
        {
            UserId = "";
            Email = "";
            FullName = "";
            SignedAt = DateTime.UtcNow;
            TokenType = "Bearer";
            Token = "";
        }
    }
}