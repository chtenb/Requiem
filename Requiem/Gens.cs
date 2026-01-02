using CsCheck;
using Requiem.Generators;

namespace Requiem;

/// <summary>
/// Factory for creating generators with built-in edge case bias.
/// Provides pre-configured generators for common types with comprehensive edge case coverage.
/// </summary>
public static partial class Gens
{
    /// <summary>
    /// Integer generator with compound distribution including edge cases, powers of 2, and normal values.
    /// Biased towards: 0, ±1, ±2, int.Min/MaxValue, powers of 2, and boundary values.
    /// </summary>
    public static Generator<int> Int => new(BiasedNumbers.Int);

    /// <summary>
    /// Long generator with compound distribution including edge cases and boundary values.
    /// Biased towards: 0, ±1, ±2, long.Min/MaxValue, powers of 2, and int boundaries.
    /// </summary>
    public static Generator<long> Long => new(BiasedNumbers.Long);

    /// <summary>
    /// Double generator with compound distribution including special values and edge cases.
    /// Biased towards: 0, ±1, NaN, ±Infinity, Epsilon, Min/MaxValue, and very small/large values.
    /// </summary>
    public static Generator<double> Double => new(BiasedNumbers.Double);

    /// <summary>
    /// Float generator with compound distribution including special values and edge cases.
    /// Biased towards: 0, ±1, NaN, ±Infinity, Epsilon, Min/MaxValue, and very small/large values.
    /// </summary>
    public static Generator<float> Float => new(BiasedNumbers.Float);

    /// <summary>
    /// Decimal generator with compound distribution including boundary values.
    /// Biased towards: 0, ±1, Min/MaxValue, and very small/large decimal values.
    /// </summary>
    public static Generator<decimal> Decimal => new(BiasedNumbers.Decimal);

    /// <summary>
    /// Boolean generator with equal probability of true and false.
    /// </summary>
    public static Generator<bool> Bool => new(Gen.Bool);

    /// <summary>
    /// Byte generator with edge cases.
    /// </summary>
    public static Generator<byte> Byte => new(Gen.Byte);

    /// <summary>
    /// Character generator.
    /// </summary>
    public static Generator<char> Char => new(Gen.Char);

    /// <summary>
    /// String generator with compound distribution including edge cases and problematic strings.
    /// Biased towards: empty, whitespace, null-like strings, very long strings, special characters, and Unicode edge cases.
    /// </summary>
    public static Generator<string> String => new(BiasedStrings.String);

    /// <summary>
    /// DateTime generator with compound distribution including problematic dates.
    /// Biased towards: Min/MaxValue, Unix epoch, Y2K, leap years, DST transitions, month/year boundaries, and midnight/noon.
    /// </summary>
    public static Generator<DateTime> DateTime => new(BiasedTemporal.DateTime);

    /// <summary>
    /// TimeSpan generator with compound distribution including boundary values and common durations.
    /// Biased towards: Zero, Min/MaxValue, common durations (1 second/minute/hour/day), negative values, and very small/large spans.
    /// </summary>
    public static Generator<TimeSpan> TimeSpan => new(BiasedTemporal.TimeSpan);

    /// <summary>
    /// DateTimeOffset generator with compound distribution including timezone edge cases.
    /// Biased towards: Min/MaxValue, Unix epoch, extreme timezones (±14 hours), and DST transitions.
    /// </summary>
    public static Generator<DateTimeOffset> DateTimeOffset => new(BiasedTemporal.DateTimeOffset);

    /// <summary>
    /// String generator biased towards dangerous/malicious input patterns.
    /// Includes: SQL injection, XSS, path traversal, format string attacks, and general edge cases.
    /// Use for security and input validation testing.
    /// </summary>
    public static Generator<string> DangerousString => new(BiasedStrings.DangerousString);

    /// <summary>
    /// File path generator with compound distribution including edge cases.
    /// Biased towards: empty, ".", "..", root paths, UNC paths, reserved names (Windows), very long paths, and special characters.
    /// </summary>
    public static Generator<string> FilePath => new(BiasedStrings.FilePath);

    /// <summary>
    /// Email address generator with compound distribution including edge cases.
    /// Biased towards: empty, malformed emails, edge cases (max length local/domain), and valid but unusual formats.
    /// </summary>
    public static Generator<string> Email => new(BiasedStrings.Email);

    /// <summary>
    /// URL generator with compound distribution including edge cases.
    /// Biased towards: empty, incomplete URLs, localhost, invalid ports, path traversal, special characters, and query strings.
    /// </summary>
    public static Generator<string> Url => new(BiasedStrings.Url);

    /// <summary>
    /// IPv4 address generator with compound distribution including edge cases.
    /// Biased towards: 0.0.0.0, 127.0.0.1, 255.255.255.255, private ranges, multicast, link-local, and invalid formats.
    /// </summary>
    public static Generator<string> IPv4 => new(BiasedStrings.IPv4);

    /// <summary>
    /// Credit card number generator (for testing only, not real cards).
    /// Biased towards: test card numbers, empty, all zeros, invalid lengths, and letters.
    /// </summary>
    public static Generator<string> CreditCardNumber => new(BiasedStrings.CreditCardNumber);

    /// <summary>
    /// Phone number generator with compound distribution including various formats.
    /// Biased towards: empty, single digit, various formats (with/without country code, parentheses, dashes), and extensions.
    /// </summary>
    public static Generator<string> PhoneNumber => new(BiasedStrings.PhoneNumber);

    /// <summary>
    /// JSON string generator with compound distribution including edge cases.
    /// Biased towards: empty objects/arrays, null, malformed JSON, deep nesting, trailing commas, and special characters.
    /// </summary>
    public static Generator<string> Json => new(BiasedStrings.Json);

    /// <summary>
    /// XML string generator with compound distribution including edge cases.
    /// Biased towards: empty, self-closing tags, malformed XML, CDATA sections, entities, and attributes.
    /// </summary>
    public static Generator<string> Xml => new(BiasedStrings.Xml);

    /// <summary>
    /// Create a generator that always returns the same constant value.
    /// </summary>
    public static Generator<T> Const<T>(T value) => 
        new(Gen.Const(value));

    /// <summary>
    /// Create a generator that randomly selects one of the provided values with equal probability.
    /// </summary>
    public static Generator<T> OneOf<T>(params T[] values) => 
        new(Gen.OneOfConst(values));

    /// <summary>
    /// Create a generator that selects from multiple generators based on weighted frequencies.
    /// Weights are automatically normalized.
    /// </summary>
    public static Generator<T> Frequency<T>(params (int Weight, Generator<T> Gen)[] choices)
    {
        if (choices.Length == 0)
            throw new ArgumentException("Must provide at least one choice", nameof(choices));
            
        var csCheckChoices = choices.Select(c => (c.Weight, (IGen<T>)c.Gen.Inner)).ToArray();
        return new(Gen.Frequency(csCheckChoices));
    }

    /// <summary>
    /// Create an integer generator within the specified range (inclusive).
    /// </summary>
    public static Generator<int> Range(int min, int max) =>
        new(Gen.Int[min, max]);

    /// <summary>
    /// Create a long generator within the specified range (inclusive).
    /// </summary>
    public static Generator<long> Range(long min, long max) =>
        new(Gen.Long[min, max]);

    /// <summary>
    /// Create a double generator within the specified range.
    /// </summary>
    public static Generator<double> Range(double min, double max) =>
        new(Gen.Double[min, max]);

    /// <summary>
    /// Create a float generator within the specified range.
    /// </summary>
    public static Generator<float> Range(float min, float max) =>
        new(Gen.Float[min, max]);

    /// <summary>
    /// Create a decimal generator within the specified range.
    /// </summary>
    public static Generator<decimal> Range(decimal min, decimal max) =>
        new(Gen.Decimal[min, max]);
}