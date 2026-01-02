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