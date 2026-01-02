using CsCheck;

namespace Requiem;

/// <summary>
/// Analysis extension methods for Generator<T>.
/// </summary>
public static partial class GeneratorExtensions
{
    /// <summary>
    /// Analyze the distribution of generated values.
    /// </summary>
    public static Dictionary<T, int> AnalyzeDistribution<T>(
        this Generator<T> gen,
        int sampleSize = 1000) where T : notnull
    {
        var distribution = new Dictionary<T, int>();
        foreach (var value in gen.Inner.Array[sampleSize].Single())
        {
            _ = distribution.TryGetValue(value, out var count);
            distribution[value] = count + 1;
        }
        return distribution;
    }
}

