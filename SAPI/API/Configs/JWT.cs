using Jil;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;

namespace API.Configs
{
    public class JWT
    {
        public string Secret { get; set; }

        public long TTL { get; set; }

        public JWT()
        {
            Secret = "";
            TTL = 1;
        }

        public string Encode(DTO.Tokens.JWT payload = null)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(payload, Secret);
        }

        public DTO.Tokens.JWT Decode(string Token = "")
        {
            if (string.IsNullOrWhiteSpace(Token))
            {
                return null;
            }

            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

            var json = decoder.Decode(Token, Secret, true);
            return JSON.Deserialize<DTO.Tokens.JWT>(json);
        }

        public int Verify(string Authorization = "")
        {
            if (string.IsNullOrWhiteSpace(Authorization))
            {
                return 0;
            }
            else
            {
                try
                {
                    var Token = Decode(Authorization);

                    return 1;
                }
                catch (TokenExpiredException tee)
                {
                    Console.WriteLine(Environment.NewLine + tee.ToString() + Environment.NewLine);
                    return -1;
                }
                catch (SignatureVerificationException sve)
                {
                    Console.WriteLine(Environment.NewLine + sve.ToString() + Environment.NewLine);
                    return -2;
                }
            }
        }
    }
}