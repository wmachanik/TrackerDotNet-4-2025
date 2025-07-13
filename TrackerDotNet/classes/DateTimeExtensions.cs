// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.classes.DateTimeExtensions
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

using System;

// #nullable disable --- not for this version of C#
namespace TrackerDotNet.classes
{

    public static class DateTimeExtensions
    {
        public static DateTime GetFirstDayOfWeek(this DateTime sourceDateTime)
        {
            var diff = sourceDateTime.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7; // Adjust for Sunday
            return sourceDateTime.AddDays(-diff).Date;
        }

        public static DateTime GetLastDayOfWeek(this DateTime sourceDateTime)
        {
            return sourceDateTime.GetFirstDayOfWeek().AddDays(6);
        }
    }
}