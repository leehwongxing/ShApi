using System;

namespace DTO.Tokens
{
    public class JWT
    {
        public string iss { get; set; }

        public string sub { get; set; } // ID of user

        public string aud { get; set; } // user type

        public long exp { get; set; }

        public long iat { get; set; }

        public string jti { get; set; } // use BsonId

        public JWT()
        {
            Setup();
        }

        public JWT(long TTL = 172800) // 2 days
        {
            Setup(TTL);
        }

        public void Setup(long TTL = 172800)
        {
            if (TTL < 0)
            {
                TTL = 172800;
            }

            iss = "SAPI";
            sub = "";
            aud = "Default";
            exp = DateTimeOffset.UtcNow.AddSeconds(TTL).ToUnixTimeSeconds();
            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            jti = Generator.Id();
        }
    }
}