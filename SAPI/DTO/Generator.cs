using MongoDB.Bson;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace DTO
{
    public class Generator
    {
        private static string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };

        public static string Id()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public static long Tick()
        {
            var Tick = (DateTimeOffset)DateTime.UtcNow;

            return Tick.ToUnixTimeSeconds();
        }

        public static string StripVietnamese(string Input = "")
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    Input = Input.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return Input;
        }

        public static string StripAccents(string Input = "")
        {
            var InputBytes = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, Encoding.UTF8.GetBytes(StripVietnamese(Input)));
            var Output = Encoding.UTF8.GetString(InputBytes).Replace("?", "");
            var Chars = "!@#$%^&*()_+|,.<>/?:;'\"[{]}";

            foreach (var c in Chars)
            {
                Output = Output.Replace(c.ToString(), string.Empty);
            }
            return Regex.Replace(Output, @"\s+", " ");
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