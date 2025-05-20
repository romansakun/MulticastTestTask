using System;

namespace Infrastructure.Extensions
{
    public static class TimeSpanExtensions 
    {
        public static string HmsD2(this TimeSpan span, string prefix = "", string suffix = "")
        {
            return $@"{prefix}{(int)span.TotalHours:D2}:{span.Minutes:D2}:{span.Seconds:D2}{suffix}";
        }
    }
}