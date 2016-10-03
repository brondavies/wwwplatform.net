using System;

namespace wwwplatform.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToTimezone (this DateTime value, int timezoneOffset)
        {
            return value.AddMinutes(timezoneOffset);
        }

        public static DateTime FromTimezone(this DateTime value, int timezoneOffset)
        {
            return value.AddMinutes(0 - timezoneOffset);
        }
    }
}
