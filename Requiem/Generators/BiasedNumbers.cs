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
        if (int.MaxValue >= min && int.MaxValue <= max) typeBoundaries.Add(int.MaxValue);
        if (int.MinValue >= min && int.MinValue <= max) typeBoundaries.Add(int.MinValue);
        if (int.MaxValue - 1 >= min && int.MaxValue - 1 <= max) typeBoundaries.Add(int.MaxValue - 1);
        if (int.MinValue + 1 >= min && int.MinValue + 1 <= max) typeBoundaries.Add(int.MinValue + 1);
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
        if (long.MaxValue >= min && long.MaxValue <= max) typeBoundaries.Add(long.MaxValue);
        if (long.MinValue >= min && long.MinValue <= max) typeBoundaries.Add(long.MinValue);
        if (long.MaxValue - 1 >= min && long.MaxValue - 1 <= max) typeBoundaries.Add(long.MaxValue - 1);
        if (long.MinValue + 1 >= min && long.MinValue + 1 <= max) typeBoundaries.Add(long.MinValue + 1);
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

    public static Gen<double> DoubleRange(double min, double max)
    {
        if (min > max)
            throw new ArgumentException("min must be <= max");

        if (min == max)
            return Gen.Const(min);

        double mid = (min + max) / 2;
        double q1 = min + (max - min) / 4;
        double q3 = min + (max - min) * 3 / 4;

        // Collect generators for each category
        IGen<double> boundaries = Gen.OneOfConst(min, max);
        
        IGen<double>? nearMin = !double.IsInfinity(min) && !double.IsInfinity(max)
            ? Gen.Double.Select(eps => min + eps * (max - min) * 0.001)
            : null;
        
        IGen<double>? nearMax = !double.IsInfinity(min) && !double.IsInfinity(max)
            ? Gen.Double.Select(eps => max - eps * (max - min) * 0.001)
            : null;

        IGen<double> midpoint = Gen.OneOfConst(mid);
        IGen<double> quarterPoints = Gen.OneOfConst(q1, q3);

        // Common special values
        var specialValues = new List<double>();
        if (0.0 >= min && 0.0 <= max) specialValues.Add(0.0);
        if (-0.0 >= min && -0.0 <= max) specialValues.Add(-0.0);
        if (1.0 >= min && 1.0 <= max) specialValues.Add(1.0);
        if (-1.0 >= min && -1.0 <= max) specialValues.Add(-1.0);
        if (2.0 >= min && 2.0 <= max) specialValues.Add(2.0);
        if (-2.0 >= min && -2.0 <= max) specialValues.Add(-2.0);
        if (0.5 >= min && 0.5 <= max) specialValues.Add(0.5);
        if (-0.5 >= min && -0.5 <= max) specialValues.Add(-0.5);
        if (0.1 >= min && 0.1 <= max) specialValues.Add(0.1);
        if (-0.1 >= min && -0.1 <= max) specialValues.Add(-0.1);
        if (0.01 >= min && 0.01 <= max) specialValues.Add(0.01);
        if (-0.01 >= min && -0.01 <= max) specialValues.Add(-0.01);
        IGen<double>? special = specialValues.Count > 0 ? Gen.OneOfConst(specialValues.ToArray()) : null;

        // Type boundary and special values
        var typeBoundaries = new List<double>();
        if (!double.IsNaN(double.NaN) && double.NaN >= min && double.NaN <= max) typeBoundaries.Add(double.NaN);
        if (double.PositiveInfinity >= min && double.PositiveInfinity <= max) typeBoundaries.Add(double.PositiveInfinity);
        if (double.NegativeInfinity >= min && double.NegativeInfinity <= max) typeBoundaries.Add(double.NegativeInfinity);
        if (double.Epsilon >= min && double.Epsilon <= max) typeBoundaries.Add(double.Epsilon);
        if (-double.Epsilon >= min && -double.Epsilon <= max) typeBoundaries.Add(-double.Epsilon);
        if (double.MaxValue >= min && double.MaxValue <= max) typeBoundaries.Add(double.MaxValue);
        if (double.MinValue >= min && double.MinValue <= max) typeBoundaries.Add(double.MinValue);
        IGen<double>? typeBounds = typeBoundaries.Count > 0 ? Gen.OneOfConst(typeBoundaries.ToArray()) : null;

        // Very small doubles - collect all valid values
        var verySmallValues = new List<double>();
        for (int exp = -300; exp <= -10; exp++)
        {
            double val = Math.Pow(10, exp);
            if (val >= min && val <= max)
                verySmallValues.Add(val);
        }
        IGen<double>? verySmall = verySmallValues.Count > 0 ? Gen.OneOfConst(verySmallValues.ToArray()) : null;

        // Very large doubles - collect all valid values
        var veryLargeValues = new List<double>();
        for (int exp = 10; exp <= 300; exp++)
        {
            double val = Math.Pow(10, exp);
            if (val >= min && val <= max)
                veryLargeValues.Add(val);
        }
        IGen<double>? veryLarge = veryLargeValues.Count > 0 ? Gen.OneOfConst(veryLargeValues.ToArray()) : null;

        // Special values from Gen.Double.Special - collect valid ones
        var specialGenValues = new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity, 
            double.Epsilon, -double.Epsilon, double.MaxValue, double.MinValue, 0.0, 1.0, -1.0, 
            0.5, -0.5, 2.0, -2.0, 10.0, -10.0, 100.0, -100.0 }
            .Where(d => !double.IsNaN(d) && d >= min && d <= max)
            .Distinct()
            .ToArray();
        IGen<double>? specialGen = specialGenValues.Length > 0 ? Gen.OneOfConst(specialGenValues) : null;

        IGen<double> uniform = Gen.Double[min, max];

        // Build final distribution
        var choices = new List<(int, IGen<double>)>
        {
            (30, boundaries),
            (10, midpoint),
            (10, quarterPoints),
            (5, uniform)
        };
        if (nearMin != null) choices.Add((15, nearMin));
        if (nearMax != null) choices.Add((15, nearMax));
        if (special != null) choices.Add((20, special));
        if (typeBounds != null) choices.Add((15, typeBounds));
        if (verySmall != null) choices.Add((10, verySmall));
        if (veryLarge != null) choices.Add((10, veryLarge));
        if (specialGen != null) choices.Add((15, specialGen));

        return Gen.Frequency(choices.ToArray());
    }

    public static Gen<float> FloatRange(float min, float max)
    {
        if (min > max)
            throw new ArgumentException("min must be <= max");

        if (min == max)
            return Gen.Const(min);

        float mid = (min + max) / 2;
        float q1 = min + (max - min) / 4;
        float q3 = min + (max - min) * 3 / 4;

        // Collect generators for each category
        IGen<float> boundaries = Gen.OneOfConst(min, max);
        
        IGen<float>? nearMin = !float.IsInfinity(min) && !float.IsInfinity(max)
            ? Gen.Float.Select(eps => min + eps * (max - min) * 0.001f)
            : null;
        
        IGen<float>? nearMax = !float.IsInfinity(min) && !float.IsInfinity(max)
            ? Gen.Float.Select(eps => max - eps * (max - min) * 0.001f)
            : null;

        IGen<float> midpoint = Gen.OneOfConst(mid);
        IGen<float> quarterPoints = Gen.OneOfConst(q1, q3);

        // Common special values
        var specialValues = new List<float>();
        if (0.0f >= min && 0.0f <= max) specialValues.Add(0.0f);
        if (-0.0f >= min && -0.0f <= max) specialValues.Add(-0.0f);
        if (1.0f >= min && 1.0f <= max) specialValues.Add(1.0f);
        if (-1.0f >= min && -1.0f <= max) specialValues.Add(-1.0f);
        if (2.0f >= min && 2.0f <= max) specialValues.Add(2.0f);
        if (-2.0f >= min && -2.0f <= max) specialValues.Add(-2.0f);
        if (0.5f >= min && 0.5f <= max) specialValues.Add(0.5f);
        if (-0.5f >= min && -0.5f <= max) specialValues.Add(-0.5f);
        if (0.1f >= min && 0.1f <= max) specialValues.Add(0.1f);
        if (-0.1f >= min && -0.1f <= max) specialValues.Add(-0.1f);
        if (0.01f >= min && 0.01f <= max) specialValues.Add(0.01f);
        if (-0.01f >= min && -0.01f <= max) specialValues.Add(-0.01f);
        IGen<float>? special = specialValues.Count > 0 ? Gen.OneOfConst(specialValues.ToArray()) : null;

        // Type boundary and special values
        var typeBoundaries = new List<float>();
        if (!float.IsNaN(float.NaN) && float.NaN >= min && float.NaN <= max) typeBoundaries.Add(float.NaN);
        if (float.PositiveInfinity >= min && float.PositiveInfinity <= max) typeBoundaries.Add(float.PositiveInfinity);
        if (float.NegativeInfinity >= min && float.NegativeInfinity <= max) typeBoundaries.Add(float.NegativeInfinity);
        if (float.Epsilon >= min && float.Epsilon <= max) typeBoundaries.Add(float.Epsilon);
        if (-float.Epsilon >= min && -float.Epsilon <= max) typeBoundaries.Add(-float.Epsilon);
        if (float.MaxValue >= min && float.MaxValue <= max) typeBoundaries.Add(float.MaxValue);
        if (float.MinValue >= min && float.MinValue <= max) typeBoundaries.Add(float.MinValue);
        IGen<float>? typeBounds = typeBoundaries.Count > 0 ? Gen.OneOfConst(typeBoundaries.ToArray()) : null;

        // Very small floats - collect all valid values
        var verySmallValues = new List<float>();
        for (int exp = -38; exp <= -10; exp++)
        {
            float val = (float)Math.Pow(10, exp);
            if (val >= min && val <= max)
                verySmallValues.Add(val);
        }
        IGen<float>? verySmall = verySmallValues.Count > 0 ? Gen.OneOfConst(verySmallValues.ToArray()) : null;

        // Very large floats - collect all valid values
        var veryLargeValues = new List<float>();
        for (int exp = 10; exp <= 38; exp++)
        {
            float val = (float)Math.Pow(10, exp);
            if (val >= min && val <= max)
                veryLargeValues.Add(val);
        }
        IGen<float>? veryLarge = veryLargeValues.Count > 0 ? Gen.OneOfConst(veryLargeValues.ToArray()) : null;

        // Special values from Gen.Float.Special - collect valid ones
        var specialGenValues = new[] { float.NaN, float.PositiveInfinity, float.NegativeInfinity, 
            float.Epsilon, -float.Epsilon, float.MaxValue, float.MinValue, 0.0f, 1.0f, -1.0f, 
            0.5f, -0.5f, 2.0f, -2.0f, 10.0f, -10.0f, 100.0f, -100.0f }
            .Where(f => !float.IsNaN(f) && f >= min && f <= max)
            .Distinct()
            .ToArray();
        IGen<float>? specialGen = specialGenValues.Length > 0 ? Gen.OneOfConst(specialGenValues) : null;

        IGen<float> uniform = Gen.Float[min, max];

        // Build final distribution
        var choices = new List<(int, IGen<float>)>
        {
            (30, boundaries),
            (10, midpoint),
            (10, quarterPoints),
            (5, uniform)
        };
        if (nearMin != null) choices.Add((15, nearMin));
        if (nearMax != null) choices.Add((15, nearMax));
        if (special != null) choices.Add((20, special));
        if (typeBounds != null) choices.Add((15, typeBounds));
        if (verySmall != null) choices.Add((10, verySmall));
        if (veryLarge != null) choices.Add((10, veryLarge));
        if (specialGen != null) choices.Add((15, specialGen));

        return Gen.Frequency(choices.ToArray());
    }

    public static Gen<decimal> DecimalRange(decimal min, decimal max)
    {
        if (min > max)
            throw new ArgumentException("min must be <= max");

        if (min == max)
            return Gen.Const(min);

        decimal mid = (min + max) / 2;
        decimal q1 = min + (max - min) / 4;
        decimal q3 = min + (max - min) * 3 / 4;

        // Collect generators for each category
        IGen<decimal> boundaries = Gen.OneOfConst(min, max);
        IGen<decimal> nearMin = Gen.Decimal.Select(eps => min + eps * (max - min) * 0.001m);
        IGen<decimal> nearMax = Gen.Decimal.Select(eps => max - eps * (max - min) * 0.001m);
        IGen<decimal> midpoint = Gen.OneOfConst(mid);
        IGen<decimal> quarterPoints = Gen.OneOfConst(q1, q3);

        // Common special values
        var specialValues = new List<decimal>();
        if (0m >= min && 0m <= max) specialValues.Add(0m);
        if (1m >= min && 1m <= max) specialValues.Add(1m);
        if (-1m >= min && -1m <= max) specialValues.Add(-1m);
        if (2m >= min && 2m <= max) specialValues.Add(2m);
        if (-2m >= min && -2m <= max) specialValues.Add(-2m);
        if (0.5m >= min && 0.5m <= max) specialValues.Add(0.5m);
        if (-0.5m >= min && -0.5m <= max) specialValues.Add(-0.5m);
        if (0.1m >= min && 0.1m <= max) specialValues.Add(0.1m);
        if (-0.1m >= min && -0.1m <= max) specialValues.Add(-0.1m);
        if (0.01m >= min && 0.01m <= max) specialValues.Add(0.01m);
        if (-0.01m >= min && -0.01m <= max) specialValues.Add(-0.01m);
        if (0.001m >= min && 0.001m <= max) specialValues.Add(0.001m);
        if (-0.001m >= min && -0.001m <= max) specialValues.Add(-0.001m);
        IGen<decimal>? special = specialValues.Count > 0 ? Gen.OneOfConst(specialValues.ToArray()) : null;

        // Type boundary values
        var typeBoundaries = new List<decimal>();
        if (decimal.MaxValue >= min && decimal.MaxValue <= max) typeBoundaries.Add(decimal.MaxValue);
        if (decimal.MinValue >= min && decimal.MinValue <= max) typeBoundaries.Add(decimal.MinValue);
        if (decimal.One >= min && decimal.One <= max) typeBoundaries.Add(decimal.One);
        if (decimal.MinusOne >= min && decimal.MinusOne <= max) typeBoundaries.Add(decimal.MinusOne);
        if (decimal.Zero >= min && decimal.Zero <= max) typeBoundaries.Add(decimal.Zero);
        IGen<decimal>? typeBounds = typeBoundaries.Count > 0 ? Gen.OneOfConst(typeBoundaries.ToArray()) : null;

        // Very small decimals - collect all valid values
        var verySmallValues = new List<decimal>();
        for (int exp = -28; exp <= -10; exp++)
        {
            decimal val = (decimal)Math.Pow(10, exp);
            if (val >= min && val <= max)
                verySmallValues.Add(val);
        }
        IGen<decimal>? verySmall = verySmallValues.Count > 0 ? Gen.OneOfConst(verySmallValues.ToArray()) : null;

        // Very large decimals - collect all valid values
        var veryLargeValues = new List<decimal>();
        for (int exp = 10; exp <= 28; exp++)
        {
            decimal val = (decimal)Math.Pow(10, exp);
            if (val >= min && val <= max)
                veryLargeValues.Add(val);
        }
        IGen<decimal>? veryLarge = veryLargeValues.Count > 0 ? Gen.OneOfConst(veryLargeValues.ToArray()) : null;

        IGen<decimal> uniform = Gen.Decimal[min, max];

        // Build final distribution
        var choices = new List<(int, IGen<decimal>)>
        {
            (30, boundaries),
            (15, nearMin),
            (15, nearMax),
            (10, midpoint),
            (10, quarterPoints),
            (5, uniform)
        };
        if (special != null) choices.Add((20, special));
        if (typeBounds != null) choices.Add((15, typeBounds));
        if (verySmall != null) choices.Add((10, verySmall));
        if (veryLarge != null) choices.Add((10, veryLarge));

        return Gen.Frequency(choices.ToArray());
    }
}
