using CsCheck;

namespace Requiem.Generators;

internal static class BiasedNumbers
{
    public static Gen<int> IntRange(int min, int max)
    {
        if (min > max)
            throw new ArgumentException("min must be <= max");

        if (min == max)
            return Gen.Const(min);

        // Calculate midpoint and quarter points
        long range = (long)max - min;
        int mid = (int)(min + range / 2);
        int q1 = (int)(min + range / 4);
        int q3 = (int)(min + range * 3 / 4);

        // Collect generators for each category
        IGen<int> boundaries = Gen.OneOf(min, max);
        IGen<int>? nearBoundaries = range > 1 ? Gen.OneOf(min + 1, max - 1) : null;
        IGen<int> midpoint = Gen.OneOf(mid);
        IGen<int> quarterPoints = Gen.OneOf(q1, q3);

        // Common special values
        var specialValues = new List<int>();
        if (0 >= min && 0 <= max) specialValues.Add(0);
        if (1 >= min && 1 <= max) specialValues.Add(1);
        if (-1 >= min && -1 <= max) specialValues.Add(-1);
        if (2 >= min && 2 <= max) specialValues.Add(2);
        if (-2 >= min && -2 <= max) specialValues.Add(-2);
        if (10 >= min && 10 <= max) specialValues.Add(10);
        if (-10 >= min && -10 <= max) specialValues.Add(-10);
        if (100 >= min && 100 <= max) specialValues.Add(100);
        if (-100 >= min && -100 <= max) specialValues.Add(-100);
        if (1000 >= min && 1000 <= max) specialValues.Add(1000);
        IGen<int>? special = specialValues.Count > 0 ? Gen.OneOf(specialValues.ToArray()) : null;

        // Type boundary values
        var typeBoundaries = new List<int>();
        if (byte.MaxValue >= min && byte.MaxValue <= max) typeBoundaries.Add(byte.MaxValue);
        if ((int)byte.MinValue >= min && (int)byte.MinValue <= max) typeBoundaries.Add((int)byte.MinValue);
        if (short.MaxValue >= min && short.MaxValue <= max) typeBoundaries.Add(short.MaxValue);
        if (short.MinValue >= min && short.MinValue <= max) typeBoundaries.Add(short.MinValue);
        IGen<int>? typeBounds = typeBoundaries.Count > 0 ? Gen.OneOf(typeBoundaries.ToArray()) : null;

        // Powers of two
        var powersOfTwo = new List<int>();
        for (int exp = 0; exp <= 30; exp++)
        {
            int pow = 1 << exp;
            if (pow >= min && pow <= max) powersOfTwo.Add(pow);
            if (-pow >= min && -pow <= max) powersOfTwo.Add(-pow);
        }
        IGen<int>? powers = powersOfTwo.Count > 0 ? Gen.OneOf(powersOfTwo.ToArray()) : null;

        // Near powers of two - collect all valid values
        var nearPowersOfTwo = new List<int>();
        for (int exp = 0; exp <= 30; exp++)
        {
            for (int offset = -2; offset <= 2; offset++)
            {
                int val = (1 << exp) + offset;
                if (val >= min && val <= max && !powersOfTwo.Contains(val))
                    nearPowersOfTwo.Add(val);
            }
        }
        IGen<int>? nearPowers = nearPowersOfTwo.Count > 0 ? Gen.OneOf(nearPowersOfTwo.ToArray()) : null;

        // Uniform over [min, max]; CsCheck maps the full int range to Gen.Int.Uniform
        IGen<int> uniform = Gen.Int[min, max];

        // Build final distribution
        var choices = new List<(int, IGen<int>)>
        {
            (30, boundaries),
            (10, midpoint),
            (10, quarterPoints),
            (5, uniform)
        };
        if (nearBoundaries != null) choices.Add((15, nearBoundaries));
        if (special != null) choices.Add((20, special));
        if (typeBounds != null) choices.Add((15, typeBounds));
        if (powers != null) choices.Add((10, powers));
        if (nearPowers != null) choices.Add((10, nearPowers));

        return Gen.Frequency(choices.ToArray());
    }

    public static Gen<long> LongRange(long min, long max)
    {
        if (min > max)
            throw new ArgumentException("min must be <= max");

        if (min == max)
            return Gen.Const(min);

        // Use BigInteger for safe midpoint calculation
        var range = (System.Numerics.BigInteger)max - min;
        long mid = (long)(min + range / 2);
        long q1 = (long)(min + range / 4);
        long q3 = (long)(min + range * 3 / 4);

        // Collect generators for each category
        IGen<long> boundaries = Gen.OneOf(min, max);
        IGen<long>? nearBoundaries = range > 1 ? Gen.OneOf(min + 1, max - 1) : null;
        IGen<long> midpoint = Gen.OneOf(mid);
        IGen<long> quarterPoints = Gen.OneOf(q1, q3);

        // Common special values
        var specialValues = new List<long>();
        if (0L >= min && 0L <= max) specialValues.Add(0L);
        if (1L >= min && 1L <= max) specialValues.Add(1L);
        if (-1L >= min && -1L <= max) specialValues.Add(-1L);
        if (2L >= min && 2L <= max) specialValues.Add(2L);
        if (-2L >= min && -2L <= max) specialValues.Add(-2L);
        if (10L >= min && 10L <= max) specialValues.Add(10L);
        if (-10L >= min && -10L <= max) specialValues.Add(-10L);
        if (100L >= min && 100L <= max) specialValues.Add(100L);
        if (-100L >= min && -100L <= max) specialValues.Add(-100L);
        if (1000L >= min && 1000L <= max) specialValues.Add(1000L);
        IGen<long>? special = specialValues.Count > 0 ? Gen.OneOf(specialValues.ToArray()) : null;

        // Type boundary values
        var typeBoundaries = new List<long>();
        if (int.MaxValue >= min && int.MaxValue <= max) typeBoundaries.Add(int.MaxValue);
        if ((long)int.MinValue >= min && (long)int.MinValue <= max) typeBoundaries.Add((long)int.MinValue);
        IGen<long>? typeBounds = typeBoundaries.Count > 0 ? Gen.OneOf(typeBoundaries.ToArray()) : null;

        // Powers of two
        var powersOfTwo = new List<long>();
        for (int exp = 0; exp <= 62; exp++)
        {
            long pow = 1L << exp;
            if (pow >= min && pow <= max) powersOfTwo.Add(pow);
            if (-pow >= min && -pow <= max) powersOfTwo.Add(-pow);
        }
        IGen<long>? powers = powersOfTwo.Count > 0 ? Gen.OneOf(powersOfTwo.ToArray()) : null;

        // Near powers of two - collect all valid values
        var nearPowersOfTwo = new List<long>();
        for (int exp = 0; exp <= 62; exp++)
        {
            for (int offset = -2; offset <= 2; offset++)
            {
                long val = (1L << exp) + offset;
                if (val >= min && val <= max && !powersOfTwo.Contains(val))
                    nearPowersOfTwo.Add(val);
            }
        }
        IGen<long>? nearPowers = nearPowersOfTwo.Count > 0 ? Gen.OneOf(nearPowersOfTwo.ToArray()) : null;

        // Uniform over [min, max]; CsCheck maps the full long range to Gen.Long.Uniform
        IGen<long> uniform = Gen.Long[min, max];

        // Build final distribution
        var choices = new List<(int, IGen<long>)>
        {
            (30, boundaries),
            (10, midpoint),
            (10, quarterPoints),
            (5, uniform)
        };
        if (nearBoundaries != null) choices.Add((15, nearBoundaries));
        if (special != null) choices.Add((20, special));
        if (typeBounds != null) choices.Add((15, typeBounds));
        if (powers != null) choices.Add((10, powers));
        if (nearPowers != null) choices.Add((10, nearPowers));

        return Gen.Frequency(choices.ToArray());
    }

    public static Gen<double> DoubleRange(double min, double max, bool includeNan)
    {
        if (double.IsNaN(min) || double.IsNaN(max))
            throw new ArgumentException("min and max must not be NaN");

        if (min > max)
            throw new ArgumentException("min must be <= max");

        if (min == max)
            return Gen.Const(min);

        // Finite span of the range. Infinite bounds are only ever produced as boundary values,
        // all arithmetic below uses lo/hi so it can neither overflow nor produce NaN (-inf + inf).
        double lo = Math.Max(min, double.MinValue);
        double hi = Math.Min(max, double.MaxValue);

        // Weighted sums instead of lo + (hi - lo) * k: hi - lo overflows for wide ranges
        double mid = lo * 0.5 + hi * 0.5;
        double q1 = lo * 0.75 + hi * 0.25;
        double q3 = lo * 0.25 + hi * 0.75;

        IGen<double> midpoint = Gen.OneOf(mid);
        IGen<double> quarterPoints = Gen.OneOf(q1, q3);

        // Common values
        var commonValues = new List<double>();
        if (0.0 >= min && 0.0 <= max) commonValues.Add(0.0);
        if (1.0 >= min && 1.0 <= max) commonValues.Add(1.0);
        if (-1.0 >= min && -1.0 <= max) commonValues.Add(-1.0);
        if (2.0 >= min && 2.0 <= max) commonValues.Add(2.0);
        if (-2.0 >= min && -2.0 <= max) commonValues.Add(-2.0);
        if (0.5 >= min && 0.5 <= max) commonValues.Add(0.5);
        if (-0.5 >= min && -0.5 <= max) commonValues.Add(-0.5);
        if (0.1 >= min && 0.1 <= max) commonValues.Add(0.1);
        if (-0.1 >= min && -0.1 <= max) commonValues.Add(-0.1);
        if (0.01 >= min && 0.01 <= max) commonValues.Add(0.01);
        if (-0.01 >= min && -0.01 <= max) commonValues.Add(-0.01);
        if (-0.0 >= min && -0.0 <= max) commonValues.Add(-0.0);
        IGen<double>? common = commonValues.Count > 0 ? Gen.OneOf(commonValues.ToArray()) : null;

        IGen<double> boundaries = Gen.OneOf(min, max);

        // Adjacent representable values; BitIncrement(-inf) is MinValue and BitDecrement(+inf) is MaxValue.
        // (min + double.Epsilon rounds back to min for any non-subnormal min.)
        IGen<double> nearBoundaries = Gen.OneOf(Math.BitIncrement(min), Math.BitDecrement(max));

        // Type boundary and special values
        var specialValues = new List<double>();
        if (includeNan) specialValues.Add(double.NaN);
        if (double.PositiveInfinity >= min && double.PositiveInfinity <= max) specialValues.Add(double.PositiveInfinity);
        if (double.NegativeInfinity >= min && double.NegativeInfinity <= max) specialValues.Add(double.NegativeInfinity);
        if (double.Epsilon >= min && double.Epsilon <= max) specialValues.Add(double.Epsilon);
        if (-double.Epsilon >= min && -double.Epsilon <= max) specialValues.Add(-double.Epsilon);
        if (double.MaxValue >= min && double.MaxValue <= max) specialValues.Add(double.MaxValue);
        if (double.MinValue >= min && double.MinValue <= max) specialValues.Add(double.MinValue);
        IGen<double>? typeBounds = specialValues.Count > 0 ? Gen.OneOf(specialValues.ToArray()) : null;

        // Very small doubles - collect all valid values
        var verySmallValues = new List<double>();
        for (int exp = -300; exp <= -10; exp++)
        {
            double val = Math.Pow(10, exp);
            if (val >= min && val <= max)
                verySmallValues.Add(val);
            if (-val >= min && -val <= max)
                verySmallValues.Add(-val);
        }
        IGen<double>? verySmall = verySmallValues.Count > 0 ? Gen.OneOf(verySmallValues.ToArray()) : null;

        // Very large doubles - collect all valid values
        var veryLargeValues = new List<double>();
        for (int exp = 10; exp <= 300; exp++)
        {
            double val = Math.Pow(10, exp);
            if (val >= min && val <= max)
                veryLargeValues.Add(val);
            if (-val >= min && -val <= max)
                veryLargeValues.Add(-val);
        }
        IGen<double>? veryLarge = veryLargeValues.Count > 0 ? Gen.OneOf(veryLargeValues.ToArray()) : null;

        // Uniform by value over [lo, hi]. Not Gen.Double[min, max]: that is CsCheck's own biased mix
        // (integers, rationals, exponentials) and yields NaN for infinite bounds.
        // The convex combination cannot overflow; the clamp absorbs rounding at the edges.
        IGen<double> uniform = Gen.Double.Unit.Select(u => Math.Clamp((1 - u) * lo + u * hi, lo, hi));

        // Build final distribution
        var choices = new List<(int, IGen<double>)>
        {
            (30, boundaries),
            (10, nearBoundaries),
            (10, midpoint),
            (10, quarterPoints),
            (5, uniform)
        };
        if (common != null) choices.Add((20, common));
        if (typeBounds != null) choices.Add((15, typeBounds));
        if (verySmall != null) choices.Add((10, verySmall));
        if (veryLarge != null) choices.Add((10, veryLarge));

        return Gen.Frequency(choices.ToArray());
    }
}
