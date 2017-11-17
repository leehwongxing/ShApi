using MongoDB.Bson;
using System;

namespace DTO
{
    public class Generator
    {
        public static string Id()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public static string Ansi()
        {
            return string.Join("__",
                "");
        }

        public static long Tick()
        {
            return DateTime.UtcNow.Ticks;
        }
    }
}