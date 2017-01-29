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
        private static bool usingWeeks = false;

        public static bool UseWeeks(this TimeSpan timeSpan, bool? useWeeks = null)
        {
            if (useWeeks != null)
            {
                usingWeeks = (bool)useWeeks;
            }

            return usingWeeks;
        }

        public static int Weeks(this TimeSpan timeSpan)
        {
            return usingWeeks ? (int)(timeSpan.Ticks / TicksPerWeek) : 0;
        }

        public static int Days(this TimeSpan timeSpan)
        {
            int days = (int)(timeSpan.Ticks / TimeSpan.TicksPerDay);
            return usingWeeks ? (days % 7) : days;
        }
    }
}
