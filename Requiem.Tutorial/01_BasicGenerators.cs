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