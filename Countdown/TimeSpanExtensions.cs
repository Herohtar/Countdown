using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countdown
{
    public static class TimeSpanExtensions
    {
        private const long TicksPerWeek = TimeSpan.TicksPerDay * 7;
        private static Units minimumUnits = Units.Milliseconds;
        private static Units maximumUnits = Units.Weeks;

        public static Units MinimumUnits(this TimeSpan timeSpan, Units? units)
        {
            if (units != null)
            {
                minimumUnits = (Units)units;
            }

            return minimumUnits;
        }

        public static Units MaximumUnits(this TimeSpan timeSpan, Units? units)
        {
            if (units != null)
            {
                maximumUnits = (Units)units;
            }

            return maximumUnits;
        }

        private static double getUnits(TimeSpan timeSpan, Units units)
        {
            double value = 0;

            switch (units)
            {
                case Units.Weeks:
                    value = (int)(timeSpan.Ticks / TicksPerWeek);
                    break;
                case Units.Days:
                    value = (Units.Days < maximumUnits) ? (timeSpan.Days % 7) : timeSpan.Days;
                    break;
                case Units.Hours:
                    value = (Units.Hours < maximumUnits) ? timeSpan.Hours : Math.Floor(timeSpan.TotalHours);
                    break;
                case Units.Minutes:
                    value = (Units.Minutes < maximumUnits) ? timeSpan.Minutes : Math.Floor(timeSpan.TotalMinutes);
                    break;
                case Units.Seconds:
                    value = (Units.Seconds < maximumUnits) ? timeSpan.Seconds : Math.Floor(timeSpan.TotalSeconds);
                    break;
                case Units.Milliseconds:
                    value = (Units.Milliseconds < maximumUnits) ? timeSpan.Milliseconds : Math.Floor(timeSpan.TotalMilliseconds);
                    break;
            }

            return value;
        }

        public static string FormattedDifference(this TimeSpan timeSpan, string defaultText)
        {
            List<string> timeStrings = new List<string>();

            int requestedMinimum = (int)minimumUnits;
            for (int i = (int)maximumUnits; i >= requestedMinimum; i--)
            {
                double value = getUnits(timeSpan, (Units)i);
                if ((value > 0) || (timeStrings.Count > 0))
                {
                    timeStrings.Add(formatUnits(value, (Units)i));
                }

                if ((i == requestedMinimum) && (timeStrings.Count == 0))
                {
                    requestedMinimum = Math.Max(0, requestedMinimum - 1);
                }
            }

            return (timeStrings.Count > 0) ? String.Join(", ", timeStrings) : defaultText;
        }

        private static string formatUnits(double count, Units units)
        {
            string unit = units.ToString().TrimEnd('s').ToLower();
            if (unit == "millisecond")
            {
                return String.Format("{0:000} {1}{2}", count, unit, (count == -1) ? "" : "s");
            }

            return String.Format("{0} {1}{2}", count, unit, (count == 1) ? "" : "s");
        }
    }
}
