using System;

namespace Ocluse.LiquidSnow.Extensions
{
    /// <summary>
    /// Adds extensions to <see cref="DateTime"/>
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Rounds up a date time by the provided factor.
        /// </summary>
        /// <param name="dateTime">The date time to round.</param>
        /// <param name="factor">The factor to round the date time up by, e.g 5 minutes rounds up 9.42PM to 9.45PM.</param>
        public static DateTime RoundUp(this DateTime dateTime, TimeSpan factor)
        {
            var modTicks = dateTime.Ticks % factor.Ticks;
            var delta = modTicks != 0 ? factor.Ticks - modTicks : 0;
            return new DateTime(dateTime.Ticks + delta, dateTime.Kind);
        }

        /// <summary>
        /// Rounds down a date time by the provided factor.
        /// </summary>
        /// <param name="dateTime">The date time to round.</param>
        /// <param name="factor">The factor to round the date time down by, e.g 5 minutes rounds down 9.42PM to 9.40PM.</param>
        public static DateTime RoundDown(this DateTime dateTime, TimeSpan factor)
        {
            var delta = dateTime.Ticks % factor.Ticks;
            return new DateTime(dateTime.Ticks - delta, dateTime.Kind);
        }

        /// <summary>
        /// Rounds a date time to the nearest factor.
        /// </summary>
        /// <param name="dateTime">The date time to round</param>
        /// <param name="factor">The factor to round the date time by, e.g 5 minutes rounds 9.42PM to 9.40PM</param>
        public static DateTime RoundToNearest(this DateTime dateTime, TimeSpan factor)
        {
            var delta = dateTime.Ticks % factor.Ticks;
            bool roundUp = delta > factor.Ticks / 2;
            var offset = roundUp ? factor.Ticks : 0;

            return new DateTime(dateTime.Ticks + offset - delta, dateTime.Kind);
        }

        /// <summary>
        /// Gets the week range of a DateTime, i.e the first day of that date's week and the last.
        /// </summary>
        /// <param name="startDate">The date whose week is to be obtained.</param>
        /// <param name="firstDay">The day to be used as first day of the week</param>
        /// <param name="weekRange">The number of days  in the week.</param>
        /// <returns>A tuple containing the first and last day the week where date time object is found.</returns>
        public static Tuple<DateTime, DateTime> GetWeek(this DateTime startDate, DayOfWeek firstDay = DayOfWeek.Monday, int weekRange = 7)
        {
            while (startDate.DayOfWeek != firstDay)
            {
                startDate = startDate.AddDays(-1);
            }

            DateTime endDate = startDate.AddDays(weekRange);

            return Tuple.Create(startDate, endDate);
        }
    }
}
