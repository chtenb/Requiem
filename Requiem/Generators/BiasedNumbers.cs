using CsCheck;

namespace Requiem.Generators;

/// <summary>
/// Biased generators for numeric types (int, long, double, float, decimal).
/// Heavily skewed towards edge cases and boundary values.
/// </summary>
internal static class BiasedNumbers
{
    /// <summary>
    /// Integer generator heavily skewed towards common edge cases.
    /// </summary>
    public static readonly Gen<int> Int = Gen.Frequency(
        (40, Gen.OneOfConst(0, 1, -1, 2, -2, 10, -10, 100, -100)),
        (20, Gen.OneOfConst(int.MaxValue, int.MinValue, int.MaxValue - 1, int.MinValue + 1)),
        (15, Gen.OneOfConst(byte.MaxValue, (int)byte.MinValue, short.MaxValue, short.MinValue)),
        (10, PowersOfTwo()),
        (10, NearPowersOfTwo()),
        (5, Gen.Int)
    );

    /// <summary>
    /// Long generator heavily skewed towards edge cases.
    /// </summary>
    public static readonly Gen<long> Long = Gen.Frequency(
        (40, Gen.OneOfConst(0L, 1L, -1L, 2L, -2L, 10L, -10L, 100L, -100L)),
        (20, Gen.OneOfConst(long.MaxValue, long.MinValue, long.MaxValue - 1, long.MinValue + 1)),
        (15, Gen.OneOfConst(int.MaxValue, (long)int.MinValue)),
        (10, PowersOfTwoLong()),
        (10, NearPowersOfTwoLong()),
        (5, Gen.Long)
    );

    /// <summary>
    /// Double generator with special values and edge cases.
    /// </summary>
    public static readonly Gen<double> Double = Gen.Frequency(
        (30, Gen.OneOfConst(
            0.0, -0.0, 1.0, -1.0, 2.0, -2.0,
            double.NaN, double.PositiveInfinity, double.NegativeInfinity,
            double.Epsilon, -double.Epsilon,
            double.MaxValue, double.MinValue
        )),
        (20, Gen.OneOfConst(0.5, -0.5, 0.1, -0.1, 0.01, -0.01)),
        (15, Gen.Double.Special),
        (10, VerySmallDoubles()),
        (10, VeryLargeDoubles()),
        (15, Gen.Double)
    );

    /// <summary>
    /// Float generator with special values and edge cases.
    /// </summary>
    public static readonly Gen<float> Float = Gen.Frequency(
        (30, Gen.OneOfConst(
            0.0f, -0.0f, 1.0f, -1.0f, 2.0f, -2.0f,
            float.NaN, float.PositiveInfinity, float.NegativeInfinity,
            float.Epsilon, -float.Epsilon,
            float.MaxValue, float.MinValue
        )),
        (20, Gen.OneOfConst(0.5f, -0.5f, 0.1f, -0.1f, 0.01f, -0.01f)),
        (15, Gen.Float.Special),
        (10, VerySmallFloats()),
        (10, VeryLargeFloats()),
        (15, Gen.Float)
    );

    /// <summary>
    /// Decimal generator with edge cases.
    /// </summary>
    public static readonly Gen<decimal> Decimal = Gen.Frequency(
        (30, Gen.OneOfConst(
            0m, 1m, -1m, 2m, -2m,
            decimal.MaxValue, decimal.MinValue,
            decimal.One, decimal.MinusOne, decimal.Zero
        )),
        (20, Gen.OneOfConst(0.5m, -0.5m, 0.1m, -0.1m, 0.01m, -0.01m, 0.001m, -0.001m)),
        (15, VerySmallDecimals()),
        (15, VeryLargeDecimals()),
        (20, Gen.Decimal)
    );

    private static Gen<int> PowersOfTwo() =>
        Gen.Int[0, 30].Select(exp => 1 << exp);

    private static Gen<int> NearPowersOfTwo() =>
        Gen.Int[0, 30].Select(Gen.Int[-2, 2], (exp, offset) => (1 << exp) + offset);

    private static Gen<long> PowersOfTwoLong() =>
        Gen.Int[0, 62].Select(exp => 1L << exp);

    private static Gen<long> NearPowersOfTwoLong() =>
        Gen.Int[0, 62].Select(Gen.Int[-2, 2], (exp, offset) => (1L << exp) + offset);

    private static Gen<double> VerySmallDoubles() =>
        Gen.Int[-300, -10].Select(exp => Math.Pow(10, exp));

    private static Gen<double> VeryLargeDoubles() =>
        Gen.Int[10, 300].Select(exp => Math.Pow(10, exp));

    private static Gen<float> VerySmallFloats() =>
        Gen.Int[-38, -10].Select(exp => (float)Math.Pow(10, exp));

    private static Gen<float> VeryLargeFloats() =>
        Gen.Int[10, 38].Select(exp => (float)Math.Pow(10, exp));

    private static Gen<decimal> VerySmallDecimals() =>
        Gen.Int[-28, -10].Select(exp => (decimal)Math.Pow(10, exp));

    private static Gen<decimal> VeryLargeDecimals() =>
        Gen.Int[10, 28].Select(exp => (decimal)Math.Pow(10, exp));
}