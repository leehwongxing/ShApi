using MongoDB.Bson;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

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

        public static bool IsEmail(string Email = "")
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                return false;
            }
            try
            {
                return Regex.IsMatch(Email,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}