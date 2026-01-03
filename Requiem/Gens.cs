using CsCheck;
using Requiem.Generators;

namespace Requiem;

/// <summary>
/// Factory for creating generators with built-in edge case bias.
/// Provides pre-configured generators for common types with comprehensive edge case coverage.
/// </summary>
public static partial class Gens
{
    // ============================================================================
    // Range Factory Methods
    // ============================================================================

    /// <summary>
    /// Create an integer generator within the specified range (inclusive).
    /// Heavily biased towards boundaries, midpoint, and special values within the range.
    /// </summary>
    public static Generator<int> Range(int min, int max) =>
        new(BiasedNumbers.IntRange(min, max));

    /// <summary>
    /// Create a long generator within the specified range (inclusive).
    /// Heavily biased towards boundaries, midpoint, and special values within the range.
    /// </summary>
    public static Generator<long> Range(long min, long max) =>
        new(BiasedNumbers.LongRange(min, max));

    /// <summary>
    /// Create a double generator within the specified range.
    /// Heavily biased towards boundaries, midpoint, and special values within the range.
    /// </summary>
    public static Generator<double> Range(double min, double max) =>
        new(BiasedNumbers.DoubleRange(min, max));

    /// <summary>
    /// Create a float generator within the specified range.
    /// Heavily biased towards boundaries, midpoint, and special values within the range.
    /// </summary>
    public static Generator<float> Range(float min, float max) =>
        new(BiasedNumbers.FloatRange(min, max));

    /// <summary>
    /// Create a decimal generator within the specified range.
    /// Heavily biased towards boundaries, midpoint, and special values within the range.
    /// </summary>
    public static Generator<decimal> Range(decimal min, decimal max) =>
        new(BiasedNumbers.DecimalRange(min, max));

    // ============================================================================
    // Integer Generators
    // ============================================================================

    /// <summary>
    /// Integer generator with compound distribution including edge cases, powers of 2, and normal values.
    /// Biased towards: 0, ±1, ±2, int.Min/MaxValue, powers of 2, and boundary values.
    /// </summary>
    public static Generator<int> Int => Range(int.MinValue, int.MaxValue);

    /// <summary>
    /// Positive integer generator (>= 0) with compound distribution including edge cases.
    /// Biased towards: 0, 1, 2, 10, 100, int.MaxValue, powers of 2, and boundary values.
    /// </summary>
    public static Generator<int> PositiveInt => Range(0, int.MaxValue);

    /// <summary>
    /// Long generator with compound distribution including edge cases and boundary values.
    /// Biased towards: 0, ±1, ±2, long.Min/MaxValue, powers of 2, and int boundaries.
    /// </summary>
    public static Generator<long> Long => Range(long.MinValue, long.MaxValue);

    /// <summary>
    /// Positive long generator (>= 0) with compound distribution including edge cases.
    /// Biased towards: 0, 1, 2, 10, 100, long.MaxValue, powers of 2, and boundary values.
    /// </summary>
    public static Generator<long> PositiveLong => Range(0L, long.MaxValue);

    // ============================================================================
    // Floating-Point Generators
    // ============================================================================

    /// <summary>
    /// Double generator with compound distribution including special values and edge cases.
    /// Biased towards: 0, ±1, NaN, ±Infinity, Epsilon, Min/MaxValue, and very small/large values.
    /// </summary>
    public static Generator<double> Double => Range(double.MinValue, double.MaxValue);

    /// <summary>
    /// Positive double generator (>= 0) with compound distribution including special values and edge cases.
    /// Biased towards: 0, 1, 2, +Infinity, Epsilon, MaxValue, and very small/large values.
    /// </summary>
    public static Generator<double> PositiveDouble => Range(0.0, double.MaxValue);

    /// <summary>
    /// Float generator with compound distribution including special values and edge cases.
    /// Biased towards: 0, ±1, NaN, ±Infinity, Epsilon, Min/MaxValue, and very small/large values.
    /// </summary>
    public static Generator<float> Float => Range(float.MinValue, float.MaxValue);

    /// <summary>
    /// Positive float generator (>= 0) with compound distribution including special values and edge cases.
    /// Biased towards: 0, 1, 2, +Infinity, Epsilon, MaxValue, and very small/large values.
    /// </summary>
    public static Generator<float> PositiveFloat => Range(0.0f, float.MaxValue);

    /// <summary>
    /// Decimal generator with compound distribution including boundary values.
    /// Biased towards: 0, ±1, Min/MaxValue, and very small/large decimal values.
    /// </summary>
    public static Generator<decimal> Decimal => Range(decimal.MinValue, decimal.MaxValue);

    /// <summary>
    /// Positive decimal generator (>= 0) with compound distribution including boundary values.
    /// Biased towards: 0, 1, 2, MaxValue, and very small/large decimal values.
    /// </summary>
    public static Generator<decimal> PositiveDecimal => Range(0m, decimal.MaxValue);

    // ============================================================================
    // Basic Type Generators
    // ============================================================================

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

    // ============================================================================
    // Temporal Generators
    // ============================================================================

    /// <summary>
    /// DateTime generator with compound distribution including problematic dates.
    /// Biased towards: Min/MaxValue, Unix epoch, Y2K, leap years, DST transitions, month/year boundaries, and midnight/noon.
    /// </summary>
    public static Generator<DateTime> DateTime => new(BiasedTemporal.DateTime);

    /// <summary>
    /// DateTimeOffset generator with compound distribution including timezone edge cases.
    /// Biased towards: Min/MaxValue, Unix epoch, extreme timezones (±14 hours), and DST transitions.
    /// </summary>
    public static Generator<DateTimeOffset> DateTimeOffset => new(BiasedTemporal.DateTimeOffset);

    /// <summary>
    /// TimeSpan generator with compound distribution including boundary values and common durations.
    /// Biased towards: Zero, Min/MaxValue, common durations (1 second/minute/hour/day), negative values, and very small/large spans.
    /// </summary>
    public static Generator<TimeSpan> TimeSpan => new(BiasedTemporal.TimeSpan);

    // ============================================================================
    // String Generators
    // ============================================================================

    /// <summary>
    /// String generator with compound distribution including edge cases and problematic strings.
    /// Biased towards: simple strings and naughty strings.
    /// </summary>
    /// <param name="minLength">Minimum string length (inclusive). Default is 0.</param>
    /// <param name="maxLength">Maximum string length (inclusive). Default is 100.</param>
    public static Generator<string> String(int minLength = 0, int maxLength = 100) =>
        new(BiasedStrings.String(minLength, maxLength));

    // ============================================================================
    // Specialized String Generators
    // ============================================================================

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
    /// File path generator with compound distribution including edge cases.
    /// Biased towards: empty, ".", "..", root paths, UNC paths, reserved names (Windows), very long paths, and special characters.
    /// </summary>
    public static Generator<string> FilePath => new(BiasedStrings.FilePath);

    /// <summary>
    /// Phone number generator with compound distribution including various formats.
    /// Biased towards: empty, single digit, various formats (with/without country code, parentheses, dashes), and extensions.
    /// </summary>
    public static Generator<string> PhoneNumber => new(BiasedStrings.PhoneNumber);

    /// <summary>
    /// Credit card number generator (for testing only, not real cards).
    /// Biased towards: test card numbers, empty, all zeros, invalid lengths, and letters.
    /// </summary>
    public static Generator<string> CreditCardNumber => new(BiasedStrings.CreditCardNumber);

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

    // ============================================================================
    // Distribution Generators
    // ============================================================================

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

    // ============================================================================
    // Generator Combinators and Transformers
    // ============================================================================
    
    public static Generator<T[]> Array<T>(this Generator<T> gen, int minLength = 0, int maxLength = 100) =>
        new Generator<T[]>(BiasedCollections.Array(gen.Inner, minLength, maxLength));

    public static Generator<List<T>> List<T>(this Generator<T> gen, int minLength = 0, int maxLength = 100) =>
        Array(gen, minLength, maxLength).Map(arr => new List<T>(arr));
    
    /// <summary>
    /// Transform the generated values using a selector function.
    /// Pass-through to CsCheck's Select to preserve shrinking.
    /// </summary>
    public static Generator<TResult> Map<T, TResult>(
        this Generator<T> gen,
        Func<T, TResult> selector) =>
        new(gen.Inner.Select(selector));

    /// <summary>
    /// Transform generated tuple values using a selector function with unpacked parameters.
    /// </summary>
    public static Generator<TResult> Map<T1, T2, TResult>(
        this Generator<(T1, T2)> gen,
        Func<T1, T2, TResult> selector) =>
        new(gen.Inner.Select(t => selector(t.Item1, t.Item2)));

    /// <summary>
    /// Transform generated tuple values using a selector function with unpacked parameters.
    /// </summary>
    public static Generator<TResult> Map<T1, T2, T3, TResult>(
        this Generator<(T1, T2, T3)> gen,
        Func<T1, T2, T3, TResult> selector) =>
        new(gen.Inner.Select(t => selector(t.Item1, t.Item2, t.Item3)));

    /// <summary>
    /// Transform generated tuple values using a selector function with unpacked parameters.
    /// </summary>
    public static Generator<TResult> Map<T1, T2, T3, T4, TResult>(
        this Generator<(T1, T2, T3, T4)> gen,
        Func<T1, T2, T3, T4, TResult> selector) =>
        new(gen.Inner.Select(t => selector(t.Item1, t.Item2, t.Item3, t.Item4)));

    /// <summary>
    /// Transform generated tuple values using a selector function with unpacked parameters.
    /// </summary>
    public static Generator<TResult> Map<T1, T2, T3, T4, T5, TResult>(
        this Generator<(T1, T2, T3, T4, T5)> gen,
        Func<T1, T2, T3, T4, T5, TResult> selector) =>
        new(gen.Inner.Select(t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5)));

    /// <summary>
    /// Filter generated values to only include those matching the predicate.
    /// Pass-through to CsCheck's Where to preserve shrinking.
    /// </summary>
    public static Generator<T> Filter<T>(
        this Generator<T> gen,
        Func<T, bool> predicate) =>
        new(gen.Inner.Where(predicate));

    /// <summary>
    /// Filter generated tuple values using a predicate with unpacked parameters.
    /// </summary>
    public static Generator<(T1, T2)> Filter<T1, T2>(
        this Generator<(T1, T2)> gen,
        Func<T1, T2, bool> predicate) =>
        new(gen.Inner.Where(t => predicate(t.Item1, t.Item2)));

    /// <summary>
    /// Filter generated tuple values using a predicate with unpacked parameters.
    /// </summary>
    public static Generator<(T1, T2, T3)> Filter<T1, T2, T3>(
        this Generator<(T1, T2, T3)> gen,
        Func<T1, T2, T3, bool> predicate) =>
        new(gen.Inner.Where(t => predicate(t.Item1, t.Item2, t.Item3)));

    /// <summary>
    /// Filter generated tuple values using a predicate with unpacked parameters.
    /// </summary>
    public static Generator<(T1, T2, T3, T4)> Filter<T1, T2, T3, T4>(
        this Generator<(T1, T2, T3, T4)> gen,
        Func<T1, T2, T3, T4, bool> predicate) =>
        new(gen.Inner.Where(t => predicate(t.Item1, t.Item2, t.Item3, t.Item4)));

    /// <summary>
    /// Filter generated tuple values using a predicate with unpacked parameters.
    /// </summary>
    public static Generator<(T1, T2, T3, T4, T5)> Filter<T1, T2, T3, T4, T5>(
        this Generator<(T1, T2, T3, T4, T5)> gen,
        Func<T1, T2, T3, T4, T5, bool> predicate) =>
        new(gen.Inner.Where(t => predicate(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5)));

    /// <summary>
    /// Chain this generator with another, using the value from this generator.
    /// </summary>
    public static Generator<T> Chain<T>(
        this Generator<T> gen,
        Func<T, Generator<T>> next) =>
        new(gen.Inner.SelectMany(x => next(x).Inner));

    /// <summary>
    /// Chain this tuple generator with another, using unpacked values from this generator.
    /// </summary>
    public static Generator<(T1, T2)> Chain<T1, T2>(
        this Generator<(T1, T2)> gen,
        Func<T1, T2, Generator<(T1, T2)>> next) =>
        new(gen.Inner.SelectMany(t => next(t.Item1, t.Item2).Inner));

    /// <summary>
    /// Chain this tuple generator with another, using unpacked values from this generator.
    /// </summary>
    public static Generator<(T1, T2, T3)> Chain<T1, T2, T3>(
        this Generator<(T1, T2, T3)> gen,
        Func<T1, T2, T3, Generator<(T1, T2, T3)>> next) =>
        new(gen.Inner.SelectMany(t => next(t.Item1, t.Item2, t.Item3).Inner));

    /// <summary>
    /// Chain this tuple generator with another, using unpacked values from this generator.
    /// </summary>
    public static Generator<(T1, T2, T3, T4)> Chain<T1, T2, T3, T4>(
        this Generator<(T1, T2, T3, T4)> gen,
        Func<T1, T2, T3, T4, Generator<(T1, T2, T3, T4)>> next) =>
        new(gen.Inner.SelectMany(t => next(t.Item1, t.Item2, t.Item3, t.Item4).Inner));

    /// <summary>
    /// Chain this tuple generator with another, using unpacked values from this generator.
    /// </summary>
    public static Generator<(T1, T2, T3, T4, T5)> Chain<T1, T2, T3, T4, T5>(
        this Generator<(T1, T2, T3, T4, T5)> gen,
        Func<T1, T2, T3, T4, T5, Generator<(T1, T2, T3, T4, T5)>> next) =>
        new(gen.Inner.SelectMany(t => next(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5).Inner));

    /// <summary>
    /// Generate a tuple of two values.
    /// </summary>
    public static Generator<(T, T)> Tuple<T>(this Generator<T> gen) =>
        gen.Array(2, 2).Map(arr => (arr[0], arr[1]));

    /// <summary>
    /// Generate a tuple of three values.
    /// </summary>
    public static Generator<(T, T, T)> Tuple3<T>(this Generator<T> gen) =>
        gen.Array(3, 3).Map(arr => (arr[0], arr[1], arr[2]));

    /// <summary>
    /// Generate a tuple of four values.
    /// </summary>
    public static Generator<(T, T, T, T)> Tuple4<T>(this Generator<T> gen) =>
        gen.Array(4, 4).Map(arr => (arr[0], arr[1], arr[2], arr[3]));

    /// <summary>
    /// Generate a tuple combining two different generator types.
    /// </summary>
    public static Generator<(T1, T2)> Zip<T1, T2>(
        this Generator<T1> gen1,
        Generator<T2> gen2) =>
        new(Gen.Select(gen1.Inner, gen2.Inner));

    /// <summary>
    /// Generate a tuple combining three different generator types.
    /// </summary>
    public static Generator<(T1, T2, T3)> Zip<T1, T2, T3>(
        this Generator<T1> gen1,
        Generator<T2> gen2,
        Generator<T3> gen3) =>
        new(Gen.Select(gen1.Inner, gen2.Inner, gen3.Inner));

}