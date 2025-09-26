using System.Globalization;
using System;

namespace CodingTracker.Input
{
    public class Validation
    {
        private const string DateTimeFormat = "dd-MM-yyyy HH:mm:ss";

        public static bool IsValidDateTime(string s)
        {
            if (!DateTime.TryParseExact(s, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return false;
            }

            if (result > DateTime.Now)
            {
                return false;
            }

            return true;
        }

        public static bool DateIsBefore(DateTime start, DateTime end)
        {
            return end > start;
        }

        public static bool IsValidString(string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }


        public static string GetDateTimeFormat()
        {
            return DateTimeFormat;
        }
    }
}