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
                8888,
                512 / 8
                ));
        }

        public static bool Compare(string Password, string Salt, string Hashed)
        {
            if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Hashed))
            {
                return false;
            }
            return string.Compare(Hash(Password, Salt), Hashed, true) == 0 ? true : false;
        }
    }
}