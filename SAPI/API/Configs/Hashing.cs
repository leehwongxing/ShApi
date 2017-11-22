using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

namespace API.Configs
{
    public class Hashing
    {
        public static string Hash(string Content = "", string Salt = "")
        {
            var RawSalt = Encoding.UTF8.GetBytes(Salt);

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                Content,
                RawSalt,
                KeyDerivationPrf.HMACSHA512,
                6666,
                512 / 8
                ));
        }
    }
}