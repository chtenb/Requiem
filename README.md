# Requiem

A property-based testing library for C# with built-in edge case bias. Requiem makes it easier to find bugs by automatically generating problematic values that often break code.

## Tutorial

The following tutorials demonstrate Requiem's features through complete, runnable examples.

```csharp
namespace Requiem.Tutorial;

/// <summary>
/// Requiem has edge-case-biased generators that more frequently include
/// problematic values (0, ±1, min/max, NaN, empty, etc.) to find bugs faster.
/// The biased generators excel at dimensional correlation - generating
/// identical edge cases across multiple dimensions,
/// which is essentially impossible with independent uniform random distributions.
/// </summary>
public class Tutorial
{
    /// <summary>
    /// Built-in generators are biased towards edge cases:
    /// - Gens.Int: 0, ±1, int.MinValue, int.MaxValue, powers of 2
    /// - Gens.String: empty, whitespace, very long, Unicode edge cases
    /// - Gens.Double: 0, ±1, NaN, ±Infinity, Epsilon
    /// </summary>
    [Test]
    public void BasicGeneratorsAndEdgeCases()
    {
        // Generate single values - biased towards edge cases
        var number = Gens.Int.Single();
        var text = Gens.String.Single();

        Console.WriteLine($"Int: {number}, String: '{text}'");

        // Custom generators: Range, OneOf, Frequency
        var positiveInt = Gens.Range(1, 100).Single();
        var choice = Gens.OneOf("red", "green", "blue").Single();
        var weighted = Gens.Frequency(
            (80, Gens.Const("common")),
            (20, Gens.Const("rare"))
        ).Single();

        Console.WriteLine($"Positive: {positiveInt}, Choice: {choice}, Weighted: {weighted}");
    }

    /// <summary>
    /// Verify properties hold for all generated inputs (default 1000 iterations).
    /// Automatic shrinking finds minimal counterexamples on failure.
    /// </summary>
    [Test]
    public void PropertyBasedTesting()
    {
        // Basic property: reversing twice gives original
        Gens.Int.List().Check(list =>
        {
            var reversed = list.AsEnumerable().Reverse().Reverse().ToList();
            Assert.AreEq(list, reversed);
        });

        // Custom number of iterations: run 10,000 times instead of the default 1000
        Gens.String.Check(s => Assert.IsTrue(s.Length >= 0), iter: 10000);

        // Failed tests will report the seed of the smallest counter example it found in the alotted number of iterations
        // You can provide this seed to be used as initial seed, for easier debugging, or for further shrinking of the counter example
        Gens.String.Check(s => Assert.IsTrue(s.Length >= 0), seed: "123abc");
    }

    /// <summary>
    /// Transform generators using Select, Where, and Chain.
    /// Transformations preserve shrinking behavior from CsCheck.
    /// </summary>
    [Test]
    public void GeneratorTransformations()
    {
        // Select transforms values (filter int.MinValue as Math.Abs throws)
        var positiveInts = Gens.Int.Where(x => x != int.MinValue).Select(x => Math.Abs(x));
        positiveInts.Check(x => Assert.IsTrue(x >= 0));

        // Where filters values
        var evenNumbers = Gens.Int.Where(x => x % 2 == 0);
        evenNumbers.Check(x => Assert.AreEq(x % 2, 0));

        // Multiple inputs with Zip - check commutativity of addition between double and int
        Gens.Int.Zip(Gens.Double).Check((a, b) => Assert.AreEq(a + b, b + a));

        // Chain for dependent generation - next value depends on previous
        var gen = Gens.Range(1, 10).Chain(n => Gens.Range(n, n + 10));
        gen.Check(x => Assert.IsTrue(x >= 1));

        // Combine transformations (but prefer direct generation for efficiency)
        var combined = Gens.Int
            .Where(x => x != 0 && x != int.MinValue)
            .Select(x => Math.Abs(x))
            .Where(x => x < 1000);

        var moreEfficient = Gens.Range(1, 999); // Direct is better
    }

    /// <summary>
    /// Generate collections and tuples for testing complex scenarios.
    /// Collections are biased towards edge cases: empty, single-element, large sizes, and elements that occur in multiple places.
    /// </summary>
    [Test]
    public void CollectionsAndTuples()
    {
        // Arrays with default size range [0, 100]
        Gens.Int.Array().Check(arr =>
        {
            Assert.AreEq(arr.Length, arr.Count());
            Assert.IsTrue(arr.Reverse().Reverse().SequenceEqual(arr));
        });

        // Tuples of same type
        Gens.Int.Tuple().Check((a, b) => Assert.AreEq(a + b, b + a));

        // Mixed tuples with Zip
        Gens.String.Zip(Gens.Int, Gens.Bool).Check((str, num, flag) =>
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

    /// <summary>
    /// Demonstrates dimensional correlation advantage of biased generators.
    /// Many bugs only appear when values across multiple dimensions are edge cases.
    ///
    /// In this example we generate a tuple of values from the same generator.
    /// If these were independently generated by a uniformly distributed generator,
    /// the chance of generating two similar or equal objects would be incredibly small.
    /// Our biased generators make sure that these edge cases happen much more frequently,
    /// hitting the important branches in this test.
    /// </summary>
    public static void TestEqualityLawsFor<T>(Generator<T> gen) where T : IEquatable<T>
    {
        gen.Tuple().Check((a, b) =>
        {
            // Reflexivity with edge cases
            Assert.IsTrue(a.Equals(a), $"Reflexivity failed for {a}");

            // Symmetry
            Assert.AreEq(a.Equals(b), b.Equals(a), $"Symmetry failed: {a}, {b}");

            // Consistency with object.Equals
            Assert.AreEq(a.Equals(b), Equals(a, b));

            // Hash code contract - needs dimensional correlation to test with equal edge cases
            if (a.Equals(b))
            {
                Assert.AreEq(a.GetHashCode(), b.GetHashCode(),
                    $"Hash code contract violated: {a} and {b} equal but different hashes");
            }
        });
    }
}

```

## Running the Examples

All examples are in the `Requiem.Tutorial` project. To run them:

```bash
cd lib/Requiem/Requiem.Tutorial
dotnet test
```

## Credits

Requiem is currently built on top of [CsCheck](https://github.com/AnthonyLloyd/CsCheck), an excellent property-based testing library for C#. Requiem provides a simplified API and enhanced edge case bias to make property-based testing more accessible and effective.

## License

See [LICENSE](LICENSE) file for details.