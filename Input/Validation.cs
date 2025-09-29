using System.Globalization;

namespace CodingTracker.Input
{
    public class Validation
    {
        public static bool IsValidDateTime(string dateTimeString)
        {
            string format = "yyyy-MM-dd HH:mm";
            return DateTime.TryParseExact(dateTimeString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}