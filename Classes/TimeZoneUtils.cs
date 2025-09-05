using System;
using System.Linq;
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
        public static string GetZoneAbbreviation()
        {
            switch (EffectiveTimeZone.Id)
            {
                case "South Africa Standard Time": return "SAST";
                case "GMT Standard Time": return "GMT";
                case "Greenwich Standard Time": return "GMT";
                case "W. Europe Standard Time": return "CET";
                case "Central Europe Standard Time": return "CET";
                case "Romance Standard Time": return "CET";
                case "Central European Standard Time": return "CEST";
                case "Eastern Standard Time": return "EST";
                case "Eastern Daylight Time": return "EDT";
                case "Pacific Standard Time": return "PST";
                case "Pacific Daylight Time": return "PDT";
                case "Mountain Standard Time": return "MST";
                case "Mountain Daylight Time": return "MDT";
                case "Central Standard Time": return "CST";
                case "Central Daylight Time": return "CDT";
                case "India Standard Time": return "IST";
                case "China Standard Time": return "CST";
                case "Tokyo Standard Time": return "JST";
                case "Russian Standard Time": return "MSK";
                case "Arabian Standard Time": return "AST";
                case "AUS Eastern Standard Time": return "AEST";
                case "AUS Central Standard Time": return "ACST";
                case "New Zealand Standard Time": return "NZST";
                case "UTC": return "UTC";
                case "UTC+12": return "UTC+12";
                case "UTC+10": return "UTC+10";
                case "UTC+08": return "UTC+8";
                case "UTC+03": return "UTC+3";
                case "UTC-05": return "UTC-5";
                case "UTC-08": return "UTC-8";
                // Add more as needed
                default:
                    // Fallback: use first letters of each word in the ID
                    return string.Concat(EffectiveTimeZone.Id.Split(' ').Select(w => w[0])).ToUpper();
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
