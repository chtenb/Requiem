using CsCheck;

namespace Requiem.Generators;

/// <summary>
/// Biased generators for intervals and comparison scenarios.
/// Useful for testing interval logic and edge cases in comparisons.
/// </summary>
internal static class BiasedIntervals
{
    /// <summary>
    /// Generates adjacent intervals (useful for testing interval logic).
    /// </summary>
    public static Gen<(DateTime start1, DateTime end1, DateTime start2, DateTime end2)> AdjacentDateTimeIntervals() =>
        Gen.DateTime.Select(Gen.TimeSpan, (start1, duration1) =>
        {
            var end1 = start1.Add(duration1);
            var start2 = end1;
            return Gen.TimeSpan.Select(duration2 =>
            {
                var end2 = start2.Add(duration2);
                return (start1, end1, start2, end2);
            });
        }).SelectMany(x => x);

    /// <summary>
    /// Generates overlapping intervals.
    /// </summary>
    public static Gen<(DateTime start1, DateTime end1, DateTime start2, DateTime end2)> OverlappingDateTimeIntervals() =>
        Gen.DateTime.Select(Gen.TimeSpan, Gen.TimeSpan, (start1, duration1, offset) =>
        {
            var end1 = start1.Add(duration1);
            var start2 = start1.Add(offset);
            return Gen.TimeSpan.Select(duration2 =>
            {
                var end2 = start2.Add(duration2);
                return (start1, end1, start2, end2);
            });
        }).SelectMany(x => x);

    /// <summary>
    /// Generates two numbers that are very close to each other (useful for floating point comparison tests).
    /// </summary>
    public static Gen<(double, double)> CloseDoubles(double maxDifference = 1e-10) =>
        Gen.Double.Select(Gen.Double[-maxDifference, maxDifference], (base_val, offset) =>
            (base_val, base_val + offset));

    /// <summary>
    /// Generates two strings that differ by only one character.
    /// </summary>
    public static Gen<(string, string)> StringsDifferByOneChar() =>
        Gen.String.Where(s => s.Length > 0).SelectMany(str =>
        {
            return Gen.Int[0, str.Length - 1].Select(Gen.Char, (index, newChar) =>
            {
                var chars = str.ToCharArray();
                var original = str;
                chars[index] = newChar;
                var modified = new string(chars);
                return (original, modified);
            });
        });
}