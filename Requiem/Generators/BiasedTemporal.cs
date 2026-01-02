using CsCheck;

namespace Requiem.Generators;

/// <summary>
/// Biased generators for temporal types (DateTime, TimeSpan, DateTimeOffset).
/// Heavily skewed towards edge cases and boundary values.
/// </summary>
internal static class BiasedTemporal
{
    /// <summary>
    /// DateTime generator skewed towards edge cases and problematic dates.
    /// </summary>
    public static readonly Gen<DateTime> DateTime = Gen.Frequency(
        (20, Gen.OneOfConst(
            System.DateTime.MinValue,
            System.DateTime.MaxValue,
            new DateTime(1970, 1, 1), // Unix epoch
            new DateTime(2000, 1, 1), // Y2K
            new DateTime(1900, 1, 1),
            new DateTime(2038, 1, 19), // Unix 32-bit overflow
            System.DateTime.UtcNow
        )),
        (15, LeapYearDates()),
        (15, DstTransitionDates()),
        (10, EndOfMonthDates()),
        (10, MidnightAndNoonDates()),
        (10, LeapSecondDates()),
        (10, MonthBoundaries()),
        (10, YearBoundaries()),
        (10, WeekendDates()),
        (5, February29Dates()),
        (30, Gen.DateTime)
    );

    /// <summary>
    /// TimeSpan generator skewed towards edge cases.
    /// </summary>
    public static readonly Gen<TimeSpan> TimeSpan = Gen.Frequency(
        (25, Gen.OneOfConst(
            System.TimeSpan.Zero,
            System.TimeSpan.MinValue,
            System.TimeSpan.MaxValue,
            System.TimeSpan.FromDays(1),
            System.TimeSpan.FromHours(1),
            System.TimeSpan.FromMinutes(1),
            System.TimeSpan.FromSeconds(1),
            System.TimeSpan.FromMilliseconds(1),
            System.TimeSpan.FromTicks(1)
        )),
        (15, Gen.OneOfConst(
            System.TimeSpan.FromDays(-1),
            System.TimeSpan.FromHours(-1),
            System.TimeSpan.FromMinutes(-1)
        )),
        (15, CommonDurations()),
        (10, NegativeDurations()),
        (10, MicrosecondPrecision()),
        (10, VerySmallTimeSpans()),
        (10, VeryLargeTimeSpans()),
        (40, Gen.TimeSpan)
    );

    /// <summary>
    /// DateTimeOffset generator with timezone edge cases.
    /// </summary>
    public static readonly Gen<DateTimeOffset> DateTimeOffset = Gen.Frequency(
        (20, Gen.OneOfConst(
            System.DateTimeOffset.MinValue,
            System.DateTimeOffset.MaxValue,
            System.DateTimeOffset.UtcNow,
            new DateTimeOffset(1970, 1, 1, 0, 0, 0, System.TimeSpan.Zero)
        )),
        (15, ExtremeTimezones()),
        (10, DstTransitionOffsets()),
        (55, Gen.DateTimeOffset)
    );

    private static Gen<DateTime> LeapYearDates() =>
        Gen.OneOfConst(2000, 2004, 2008, 2012, 2016, 2020, 2024)
            .Select(year => new DateTime(year, 2, 29));

    private static Gen<DateTime> DstTransitionDates() =>
        Gen.Int[2000, 2030].Select(Gen.OneOfConst(3, 10), (year, month) =>
            new DateTime(year, month, month == 3 ? 10 : 27, 2, 0, 0));

    private static Gen<DateTime> EndOfMonthDates() =>
        Gen.Int[2000, 2030].Select(Gen.Int[1, 12], (year, month) =>
            new DateTime(year, month, System.DateTime.DaysInMonth(year, month)));

    private static Gen<DateTime> MidnightAndNoonDates() =>
        Gen.DateTime.Select(dt => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour < 12 ? 0 : 12, 0, 0));

    private static Gen<DateTime> LeapSecondDates() =>
        Gen.OneOfConst(
            new DateTime(2016, 12, 31, 23, 59, 59),
            new DateTime(2015, 6, 30, 23, 59, 59),
            new DateTime(2012, 6, 30, 23, 59, 59)
        );

    private static Gen<DateTime> MonthBoundaries() =>
        Gen.Int[2000, 2030].Select(Gen.Int[1, 12], (year, month) =>
        {
            var lastDay = System.DateTime.DaysInMonth(year, month);
            return Gen.OneOfConst(
                new DateTime(year, month, 1, 0, 0, 0),
                new DateTime(year, month, lastDay, 23, 59, 59)
            );
        }).SelectMany(x => x);

    private static Gen<DateTime> YearBoundaries() =>
        Gen.Int[2000, 2030].SelectMany(year =>
            Gen.OneOfConst(
                new DateTime(year, 1, 1, 0, 0, 0),
                new DateTime(year, 12, 31, 23, 59, 59)
            ));

    private static Gen<DateTime> WeekendDates() =>
        Gen.DateTime.Select(dt =>
        {
            var daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)dt.DayOfWeek + 7) % 7;
            return dt.AddDays(daysUntilSaturday);
        });

    private static Gen<DateTime> February29Dates() =>
        Gen.OneOfConst(2000, 2004, 2008, 2012, 2016, 2020, 2024)
            .Select(year => new DateTime(year, 2, 29));

    private static Gen<TimeSpan> VerySmallTimeSpans() =>
        Gen.Long[1, 1000].Select(ticks => System.TimeSpan.FromTicks(ticks));

    private static Gen<TimeSpan> VeryLargeTimeSpans() =>
        Gen.Int[1, 365 * 100].Select(days => System.TimeSpan.FromDays(days));

    private static Gen<TimeSpan> CommonDurations() =>
        Gen.OneOfConst(
            System.TimeSpan.FromSeconds(1),
            System.TimeSpan.FromMinutes(1),
            System.TimeSpan.FromHours(1),
            System.TimeSpan.FromDays(1),
            System.TimeSpan.FromDays(7),
            System.TimeSpan.FromDays(30),
            System.TimeSpan.FromDays(365)
        );

    private static Gen<TimeSpan> NegativeDurations() =>
        CommonDurations().Select(ts => -ts);

    private static Gen<TimeSpan> MicrosecondPrecision() =>
        Gen.Long[1, 10000].Select(ticks => System.TimeSpan.FromTicks(ticks));

    private static Gen<DateTimeOffset> ExtremeTimezones() =>
        Gen.DateTime.Select(Gen.Int[-14, 14], (dt, offsetHours) =>
            new DateTimeOffset(dt, System.TimeSpan.FromHours(offsetHours)));

    private static Gen<DateTimeOffset> DstTransitionOffsets() =>
        Gen.Int[2000, 2030].Select(Gen.OneOfConst(3, 10), (year, month) =>
        {
            var day = month == 3 ? 10 : 27;
            var dt = new DateTime(year, month, day, 2, 0, 0);
            return new DateTimeOffset(dt, System.TimeSpan.FromHours(-5));
        });
}