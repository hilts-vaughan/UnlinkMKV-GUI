using System;

namespace UnlinkMKV_GUI.merge
{
    public static class TimeCodeUtil
    {
        public static TimeSpan TimeCodeToTimespan(string timeCode)
        {
            var splits = timeCode.Split(':');
            var hour = splits[0];
            var minute = splits[1];
            var second = splits[2].Split('.')[0];

            return new TimeSpan(0, int.Parse(hour), int.Parse(minute), int.Parse(second));
        }

        public static string TimespanToTimeCode(TimeSpan timespan)
        {
            var hours = timespan.Hours.ToString("D2");
            var minutes = timespan.Minutes.ToString("D2");
            var seconds = timespan.Seconds.ToString("D2");
            var milliseconds = timespan.Milliseconds.ToString(("D9"));

            return $"{hours}:{minutes}:{seconds}:{milliseconds}";
        }

    }
}