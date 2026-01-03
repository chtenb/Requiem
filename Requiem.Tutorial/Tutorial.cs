namespace Requiem.Tutorial;

/// <summary>
/// Requiem has edge-case-biased generators that more frequently include problematic values (0, ±1,
/// min/max, NaN, empty, etc.) to find bugs faster. The biased generators are better at dimensional
/// correlation than uniform generators. They can generate coinciding edge cases across multiple
/// dimensions, which is very improbable with independent uniform distributions.
/// </summary>
public class Tutorial
{
    [Test]
    public void BasicGeneratorsAndEdgeCases()
    {
        // The Built-in generators are biased towards edge cases:
        // - Gens.Int: 0, ±1, int.MinValue, int.MaxValue, powers of 2
        // - Gens.String: empty, whitespace, very long, Unicode edge cases
        // - Gens.Double: 0, ±1, NaN, ±Infinity, Epsilon

        // Generate single values
        var number = Generate.Int.Next();
        var text = Generate.String().Next();

        Console.WriteLine($"Int: {number}, String: '{text}'");

        // Generators with custom distributions
        var positiveInt = Generate.Between(1, 100).Next();
        var choice = Generate.Uniform("red", "green", "blue").Next();
        var weighted = Generate.Weighted(
            (80, Generate.Const("common")),
            (20, Generate.Const("rare"))
        ).Next();

        Console.WriteLine($"Positive: {positiveInt}, Choice: {choice}, Weighted: {weighted}");
    }

    [Test]
    public void PropertyBasedTesting()
    {
        // Verify properties hold for all generated inputs (default 1000 iterations).
        // Automatic shrinking finds minimal counterexamples on failure.

        // Basic property: reversing twice gives original
        Generate.Int.List().Check(list =>
        {
            var reversed = list.AsEnumerable().Reverse().Reverse().ToList();
            Assert.AreEq(list, reversed);
        });

        // Custom number of iterations: run 10,000 times instead of the default 1000
        Generate.String().Check(s => Assert.IsTrue(s.Length >= 0), iter: 10000);

        // Failed tests will report the seed of the smallest counter example it found in the alotted
        // number of iterations You can provide this seed to be used as initial seed, for easier
        // debugging, or for further shrinking of the counter example
        Generate.String().Check(s => Assert.IsTrue(s.Length >= 0), seed: "123456789000");

        // The properties are run in parallel on multiple threads by default, to speed up the evaluation
        // This means the generators and properties must be thread safe.
        // To make the evaluation single-threaded, set the number of threads to 1.
        Generate.String().Check(s => Assert.IsTrue(s.Length >= 0), threads: 1);
    }

    [Test]
    public void GeneratorTransformations()
    {
        // Use Map and Filter to transform generated values
        var transformed = Generate.Int
            .Filter(x => x != 0 && x != int.MinValue) // Math.Abs throws on min value
            .Map(x => Math.Abs(x))
            .Filter(x => x < 1000);

        // However, while filtering out invalid cases is often convenient in formulating generators,
        // when the invalid cases occur very often this is not very efficient. Generating the target
        // domain directly is more efficient.
        var moreEfficient = Generate.Between(1, 999);

        // Combine multiple generators with Zip
        Generate.Int.Zip(Generate.Double).Check((a, b) => Assert.AreEq(a + b, b + a));

        // Use Chain for dependent generation, where the next value depends on previous
        var gen = Generate.Between(1, 10).Chain(n => Generate.Between(n, n + 10));
        gen.Check(x => Assert.IsTrue(x >= 1));
    }

    [Test]
    public void CollectionsAndTuples()
    {
        // Collections are biased towards edge cases: empty, single-element, large sizes, and elements
        // that occur in multiple places.

        // Arrays with default size bounds [0, 100]
        Generate.Int.Array().Check(arr =>
        {
            Assert.AreEq(arr.Length, arr.Count());
            Assert.IsTrue(arr.Reverse().Reverse().SequenceEqual(arr));
        });

        // Tuples of same type
        Generate.Int.Tuple().Check((a, b) => Assert.AreEq(a + b, b + a));

        // Mixed tuples with Zip
        Generate.String().Zip(Generate.Int, Generate.Bool).Check((str, num, flag) =>
        {
            Assert.IsTrue(str != null);
            // Handle int.MinValue edge case (can't be negated)
            if (num != int.MinValue)
            {
                var result = flag ? num : -num;
                Assert.AreEq(Math.Abs(result), Math.Abs(num));
            }
        });
    }

    public static void TestEqualityLawsFor<T>(Generator<T> gen) where T : IEquatable<T>
    {
        // This example demonstrates dimensional correlation advantage of biased generators.
        // Many bugs only appear when values across multiple dimensions are edge cases.
        //
        // In this example we generate a tuple of values from the same generator.
        // If these were independently generated by a uniformly distributed generator,
        // the chance of generating two similar or equal objects would be incredibly small.
        // Our biased generators make sure that these edge cases happen much more frequently,
        // hitting the important branches in this test.

        gen.Tuple().Check((a, b) =>
        {
            // Reflexivity with edge cases
            Assert.IsTrue(a.Equals(a), $"Reflexivity failed for {a}");

            // Symmetry
            Assert.AreEq(a.Equals(b), b.Equals(a), $"Symmetry failed: {a}, {b}");

            // Consistency with object.Equals
            Assert.AreEq(a.Equals(b), Equals(a, b));

            // Consistency between GetHashCode and Equals
            if (a.Equals(b))
            {
                Assert.AreEq(a.GetHashCode(), b.GetHashCode(),
                    $"Hash code contract violated: {a} and {b} equal but different hashes");
            }
        });
    }
}
