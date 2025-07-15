using System;
using System.Web;

namespace TrackerDotNet.Classes
{
    public static class TimeZoneUtils
    {
        private static readonly string DefaultTimeZoneId =
            System.Configuration.ConfigurationManager.AppSettings["AppTimeZoneId"] ?? "South Africa Standard Time";

        private static TimeZoneInfo EffectiveTimeZone
        {
            get
            {
                if (HttpContext.Current != null &&
                    HttpContext.Current.Session != null &&
                    HttpContext.Current.Session["UserTimeZoneInfo"] is TimeZoneInfo userZone)
                {
                    return userZone;
                }

                return TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZoneId);
            }
        }

        public static DateTime Now()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EffectiveTimeZone);
        }

        public static DateTime ConvertUtcToUserZone(DateTime utcTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, EffectiveTimeZone);
        }

        public static DateTime ConvertToUtc(DateTime userLocalTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(userLocalTime, EffectiveTimeZone);
        }

        public static string GetZoneId()
        {
            return EffectiveTimeZone.Id;
        }
    }
}
