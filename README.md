# Requiem

A property-based testing library for C# with built-in edge case bias. Requiem makes it easier to find bugs by automatically generating problematic values that often break code.

## Tutorial

The following tutorials demonstrate Requiem's features through complete, runnable examples.

## Tutorial 01: Basic Generators

```csharp
namespace Requiem.Tutorial;

/// <summary>
/// Tutorial 01: Basic Generators
/// 
/// Requiem wraps CsCheck generators with a clean API and built-in edge case bias.
/// All basic generators automatically include problematic values that often break code.
/// </summary>
public class BasicGenerators
{
    [Test]
    public void SimpleGeneration()
    {
        // Generate single values for quick exploration
        var number = Gens.Int.Single();
        var text = Gens.String.Single();
        var date = Gens.DateTime.Single();
        
        Console.WriteLine($"Int: {number}, String: '{text}', DateTime: {date}");
    }

    [Test]
    public void EdgeCaseBias()
    {
        // Unlike other libraries, Requiem generators are biased towards edge cases
        // Gens.Int includes: 0, ±1, int.MinValue, int.MaxValue, powers of 2
        // Gens.String includes: empty, whitespace, very long strings, Unicode edge cases
        // Gens.Double includes: 0, ±1, NaN, ±Infinity, Epsilon
        
        // This finds bugs faster than uniform random generation
        var value = Gens.Int.Single();
        Console.WriteLine($"Generated (likely edge case): {value}");
    }

    [Test]
    public void CustomGenerators()
    {
        // Create custom generators using combinators
        var positiveInt = Gens.Range(1, 100);
        var choice = Gens.OneOf("red", "green", "blue");
        var weighted = Gens.Frequency(
            (80, Gens.Const("common")),
            (20, Gens.Const("rare"))
        );
        
        Console.WriteLine($"Positive: {positiveInt.Single()}");
        Console.WriteLine($"Choice: {choice.Single()}");
        Console.WriteLine($"Weighted: {weighted.Single()}");
    }
}
```

## Tutorial 02: Property-Based Testing

```csharp
namespace Requiem.Tutorial;

/// <summary>
/// Tutorial 02: Property-Based Testing
/// 
/// Property-based testing verifies that properties hold for all generated inputs.
/// When a property fails, the framework automatically shrinks to find minimal counterexamples.
/// </summary>
public class PropertyBasedTesting
{
    [Test]
    public void BasicPropertyCheck()
    {
        // Check that reversing a list twice gives the original list
        Gens.Int.List().Check(list =>
        {
            var reversed = list.AsEnumerable().Reverse().Reverse().ToList();
            Assert.AreEq(list, reversed);
        });
        
        // Runs 1000 iterations by default with automatic shrinking on failure
    }

    [Test]
    public void PropertyWithMultipleInputs()
    {
        // Use Zip to combine multiple generators
        var gen = Gens.Int.Zip(Gens.Int);
        
        // Check that addition is commutative
        gen.Check((a, b) =>
        {
            Assert.AreEq(a + b, b + a);
        });
    }

    [Test]
    public void ConditionalProperties()
    {
        // Use Where to filter generated values
        var positiveInts = Gens.Int.Where(x => x > 0);
        
        positiveInts.Check(x =>
        {
            Assert.IsTrue(x > 0);
            Assert.IsTrue(x * 2 > x);
        });
    }

    [Test]
    public void CustomIterations()
    {
        // Control the number of test iterations
        Gens.String.Check(s =>
        {
            // Property: string length is non-negative
            Assert.IsTrue(s.Length >= 0);
        }, iter: 10000); // Run 10,000 tests instead of default 1000
    }
}
```

## Tutorial 03: Generator Transformations

```csharp
namespace Requiem.Tutorial;

/// <summary>
/// Tutorial 03: Generator Transformations
/// 
/// Generators can be transformed using Select, Where, and Chain.
/// These preserve shrinking behavior from the underlying CsCheck framework.
/// </summary>
public class GeneratorTransformations
{
    [Test]
    public void SelectTransformation()
    {
        // Transform generated values with Select
        var positiveInts = Gens.Int.Select(x => Math.Abs(x));
        
        positiveInts.Check(x =>
        {
            Assert.IsTrue(x >= 0);
        });
    }

    [Test]
    public void WhereFiltering()
    {
        // Filter generated values with Where
        var evenNumbers = Gens.Int.Where(x => x % 2 == 0);
        
        evenNumbers.Check(x =>
        {
            Assert.AreEq(x % 2, 0);
        });
    }

    [Test]
    public void ChainDependentGeneration()
    {
        // Chain allows dependent generation - next value depends on previous
        var gen = Gens.Range(1, 10).Chain(n =>
            Gens.Range(n, n + 10) // Generate a number larger than n
        );
        
        gen.Check(x =>
        {
            Assert.IsTrue(x >= 1);
        });
    }

    [Test]
    public void CombiningTransformations()
    {
        // Transformations can be chained together
        var gen = Gens.Int
            .Where(x => x != 0)           // Filter out zero
            .Select(x => Math.Abs(x))     // Make positive
            .Where(x => x < 1000);        // Keep it reasonable
        
        gen.Check(x =>
        {
            Assert.IsTrue(x > 0);
            Assert.IsTrue(x < 1000);
        });

        // Of course this will generate a lot of cases that are later rejected
        // It is better to steer generation up front, whenever that is possible
        var moreEfficientGen = Gens.Range(1, 999);
    }
}
```

## Tutorial 04: Collections and Tuples

```csharp
namespace Requiem.Tutorial;

/// <summary>
/// Tutorial 04: Collections and Tuples
/// 
/// Generators can produce collections and tuples for testing complex scenarios.
/// Collections are biased towards edge cases like empty, single-element, and large sizes.
/// </summary>
public class CollectionsAndTuples
{
    [Test]
    public void GenerateArrays()
    {
        // Generate arrays with default size range [0, 100]
        var arrayGen = Gens.Int.Array();

        arrayGen.Check(arr =>
        {
            // Property: all elements are integers
            Assert.IsTrue(arr.All(i => 0 <= i && i <= 0));
        });
    }

    [Test]
    public void GenerateTuples()
    {
        // Generate tuples of the same type
        var pairGen = Gens.Int.Tuple();

        pairGen.Check((a, b) =>
        {
            // Property: tuple contains two integers
            Assert.AreEq(a + b, b + a);
        });
    }

    [Test]
    public void GenerateMixedTuples()
    {
        // Zip combines different generator types
        var gen = Gens.String.Zip(Gens.Int, Gens.Bool);

        gen.Check((str, num, flag) =>
        {
            // Property: tuple contains string, int, and bool
            Assert.IsTrue(str != null);
        });
    }
}
```

## Tutorial 05: Advanced Examples - Why Biased Generators Matter

```csharp
namespace Requiem.Tutorial;

/// <summary>
/// Tutorial 05: Advanced Examples - Why Biased Generators Matter
///
/// This tutorial demonstrates real-world testing scenarios where biased generators
/// excel by generating correlated edge cases across multiple dimensions.
///
/// KEY INSIGHT: Uniform random generation struggles with dimensional correlation.
/// For example, generating two equal objects requires the generator to produce
/// identical edge cases in both dimensions - essentially impossible with uniform random.
/// Biased generators solve this by correlating edge cases, making bugs discoverable.
/// </summary>
public class AdvancedExamples
{
    /// <summary>
    /// Testing Equality Laws with Dimensional Correlation
    ///
    /// This example tests that custom equality implementations satisfy fundamental laws.
    /// The challenge: many bugs only appear when BOTH objects in a tuple are edge cases.
    ///
    /// Why uniform random fails:
    /// - P(both empty arrays) ≈ very small, decreases exponentially with possible sizes
    /// - P(both at DateTime.MinValue) ≈ 1 in 10^18
    /// - P(both objects equal AND edge cases) ≈ essentially zero
    ///
    /// Why biased generators succeed:
    /// - Correlates edge cases: generates (empty, empty), (MinValue, MinValue), etc.
    /// - Tests reflexivity with actual edge case values
    /// - Finds hash code bugs that only appear with equal edge cases
    /// </summary>
    public void TestEqualityLaws<T>(Generator<T> gen)
        where T : IEquatable<T>
    {
        gen.Tuple().Check((a, b) =>
        {
            // Reflexivity: a.Equals(a) must always be true
            // Biased generator tests this with edge cases like TimeInterval.Never, TimeInterval.Always
            Assert.IsTrue(a.Equals(a), $"Reflexivity failed for {a}");

            // Symmetry: if a.Equals(b) then b.Equals(a)
            // Requires generating EQUAL objects, which needs dimensional correlation
            Assert.AreEq(a.Equals(b), b.Equals(a),
                $"Symmetry failed: {a}.Equals({b}) = {a.Equals(b)}, but {b}.Equals({a}) = {b.Equals(a)}");

            // Consistency with object.Equals operator
            Assert.AreEq(a.Equals(b), object.Equals(a, b),
                $"Equals and == operator inconsistent for {a} and {b}");

            // Hash code contract: equal objects must have equal hash codes
            // This is the critical test that needs dimensional correlation:
            // We need to generate (a, b) where a.Equals(b) is true
            // Uniform random almost never generates equal edge cases
            if (a.Equals(b))
            {
                var hashA = a.GetHashCode();
                var hashB = b.GetHashCode();
                Assert.AreEq(hashA, hashB,
                    $"Hash code contract violated: {a} and {b} are equal but have different hash codes: {hashA} vs {hashB}");
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

Requiem is built on top of [CsCheck](https://github.com/AnthonyLloyd/CsCheck), an excellent property-based testing library for C#. Requiem provides a simplified API and enhanced edge case bias to make property-based testing more accessible and effective.

## License

See [LICENSE](LICENSE) file for details.