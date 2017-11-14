using MongoDB.Bson;

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
    }
}