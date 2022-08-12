using System;

namespace CoreADPS.Helpers
{
    public class DateTimeGenerator
    {
        public static DateTime UtcNow()
        {
            var now = DateTime.UtcNow;
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        }
    }
}
