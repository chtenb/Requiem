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