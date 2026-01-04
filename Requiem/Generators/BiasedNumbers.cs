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
        int mid = min + (int)(range / 2);
        int q1 = min + (int)(range / 4);
        int q3 = min + (int)(range * 3 / 4);

        // Collect generators for each category
        IGen<int> boundaries = Gen.OneOfConst(min, max);
        IGen<int>? nearBoundaries = max - min > 1 ? Gen.OneOfConst(min + 1, max - 1) : null;
        IGen<int> midpoint = Gen.OneOfConst(mid);
        IGen<int> quarterPoints = Gen.OneOfConst(q1, q3);

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
        IGen<int>? special = specialValues.Count > 0 ? Gen.OneOfConst(specialValues.ToArray()) : null;

        // Type boundary values
        var typeBoundaries = new List<int>();
        if (byte.MaxValue >= min && byte.MaxValue <= max) typeBoundaries.Add(byte.MaxValue);
        if ((int)byte.MinValue >= min && (int)byte.MinValue <= max) typeBoundaries.Add((int)byte.MinValue);
        if (short.MaxValue >= min && short.MaxValue <= max) typeBoundaries.Add(short.MaxValue);
        if (short.MinValue >= min && short.MinValue <= max) typeBoundaries.Add(short.MinValue);
        IGen<int>? typeBounds = typeBoundaries.Count > 0 ? Gen.OneOfConst(typeBoundaries.ToArray()) : null;

        // Powers of two
        var powersOfTwo = new List<int>();
        for (int exp = 0; exp <= 30; exp++)
        {
            int pow = 1 << exp;
            if (pow >= min && pow <= max) powersOfTwo.Add(pow);
            if (-pow >= min && -pow <= max) powersOfTwo.Add(-pow);
        }
        IGen<int>? powers = powersOfTwo.Count > 0 ? Gen.OneOfConst(powersOfTwo.ToArray()) : null;

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
        IGen<int>? nearPowers = nearPowersOfTwo.Count > 0 ? Gen.OneOfConst(nearPowersOfTwo.ToArray()) : null;

        // Gen.Int throws when passing int.MinValue and int.MaxValue
        IGen<int> uniform = min == int.MinValue && max == int.MaxValue ? Gen.Int : Gen.Int[min, max];

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
        long mid = min + (long)(range / 2);
        long q1 = min + (long)(range / 4);
        long q3 = min + (long)(range * 3 / 4);

        // Collect generators for each category
        IGen<long> boundaries = Gen.OneOfConst(min, max);
        IGen<long>? nearBoundaries = max - min > 1 ? Gen.OneOfConst(min + 1, max - 1) : null;
        IGen<long> midpoint = Gen.OneOfConst(mid);
        IGen<long> quarterPoints = Gen.OneOfConst(q1, q3);

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
        IGen<long>? special = specialValues.Count > 0 ? Gen.OneOfConst(specialValues.ToArray()) : null;

        // Type boundary values
        var typeBoundaries = new List<long>();
        if (int.MaxValue >= min && int.MaxValue <= max) typeBoundaries.Add(int.MaxValue);
        if ((long)int.MinValue >= min && (long)int.MinValue <= max) typeBoundaries.Add((long)int.MinValue);
        IGen<long>? typeBounds = typeBoundaries.Count > 0 ? Gen.OneOfConst(typeBoundaries.ToArray()) : null;

        // Powers of two
        var powersOfTwo = new List<long>();
        for (int exp = 0; exp <= 62; exp++)
        {
            long pow = 1L << exp;
            if (pow >= min && pow <= max) powersOfTwo.Add(pow);
            if (-pow >= min && -pow <= max) powersOfTwo.Add(-pow);
        }
        IGen<long>? powers = powersOfTwo.Count > 0 ? Gen.OneOfConst(powersOfTwo.ToArray()) : null;

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
        IGen<long>? nearPowers = nearPowersOfTwo.Count > 0 ? Gen.OneOfConst(nearPowersOfTwo.ToArray()) : null;

        IGen<long> uniform = min == long.MinValue && max == long.MaxValue ? Gen.Long : Gen.Long[min, max];

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
        if (min > max)
            throw new ArgumentException("min must be <= max");

        if (min == max)
            return Gen.Const(min);

        double mid = (min + max) / 2;
        double q1 = min + (max - min) / 4;
        double q3 = min + (max - min) * 3 / 4;

        IGen<double> midpoint = Gen.OneOfConst(mid);
        IGen<double> quarterPoints = Gen.OneOfConst(q1, q3);

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
        IGen<double>? common = commonValues.Count > 0 ? Gen.OneOfConst(commonValues.ToArray()) : null;

        IGen<double> boundaries = Gen.OneOfConst(min, max);

        double nearMin = (double.IsNegativeInfinity(min) ? double.MinValue : min) + double.Epsilon;
        double nearMax = (double.IsInfinity(max) ? double.MaxValue : max) - double.Epsilon;
        
        IGen<double> nearBoundaries = Gen.OneOfConst(nearMin, nearMax);

        // Type boundary and special values
        var specialValues = new List<double>();
        if (includeNan) specialValues.Add(double.NaN);
        if (double.PositiveInfinity >= min && double.PositiveInfinity <= max) specialValues.Add(double.PositiveInfinity);
        if (double.NegativeInfinity >= min && double.NegativeInfinity <= max) specialValues.Add(double.NegativeInfinity);
        if (double.Epsilon >= min && double.Epsilon <= max) specialValues.Add(double.Epsilon);
        if (-double.Epsilon >= min && -double.Epsilon <= max) specialValues.Add(-double.Epsilon);
        if (double.MaxValue >= min && double.MaxValue <= max) specialValues.Add(double.MaxValue);
        if (double.MinValue >= min && double.MinValue <= max) specialValues.Add(double.MinValue);
        IGen<double>? typeBounds = specialValues.Count > 0 ? Gen.OneOfConst(specialValues.ToArray()) : null;

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
        IGen<double>? verySmall = verySmallValues.Count > 0 ? Gen.OneOfConst(verySmallValues.ToArray()) : null;

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
        IGen<double>? veryLarge = veryLargeValues.Count > 0 ? Gen.OneOfConst(veryLargeValues.ToArray()) : null;

        IGen<double> uniform = Gen.Double[min, max];

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
