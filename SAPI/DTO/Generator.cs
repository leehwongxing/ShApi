using MongoDB.Bson;
using System;
using System.Text;

namespace DTO
{
    public class Generator
    {
        public static string Id()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public static long Tick()
        {
            var Tick = (DateTimeOffset)DateTime.UtcNow;

            return Tick.ToUnixTimeSeconds();
        }

        public static string StripAccents(string Input = "")
        {
            var InputBytes = Encoding.GetEncoding(28598).GetBytes(Input);
            var Output = Encoding.UTF8.GetString(InputBytes).Replace("?", "").Replace("  ", " ");
            return Output;
        }
    }
}