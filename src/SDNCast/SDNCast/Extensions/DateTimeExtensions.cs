using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDNCast.Extensions
{
    public static class DateTimeExtensions
    {
        private const string CEST = "Central Europe Standard Time";
        private static readonly TimeZoneInfo _cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(CEST);

        public static DateTime ConvertToTimeZone(this DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
        {
            return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, destinationTimeZone);
        }

        public static DateTime ConvertFromUtcToCet(this DateTime dateTime)
        {
            return dateTime.ConvertToTimeZone(TimeZoneInfo.Utc, _cetTimeZone);
        }

        public static DateTime ConvertFromCetToUtc(this DateTime dateTime)
        {
            return dateTime.ConvertToTimeZone(_cetTimeZone, TimeZoneInfo.Utc);
        }
    }
}
