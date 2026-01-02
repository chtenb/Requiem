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