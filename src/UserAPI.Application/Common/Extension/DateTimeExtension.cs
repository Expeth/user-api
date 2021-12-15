using System;

namespace UserAPI.Application.Common.Extension
{
    public static class DateTimeExtension
    {
        public static bool IsPassed(this DateTime dateTime)
        {
            return DateTime.UtcNow > dateTime;
        }

        public static DateTime FromTimestamp(this string timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var seconds = long.Parse(timestamp);
            
            dateTime = dateTime.AddSeconds(seconds);
            return dateTime;
        }
    }
}