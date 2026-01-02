using CsCheck;
using Requiem.Generators;

namespace Requiem;


/// <summary>
/// Collection extension methods for Generator<T>
/// </summary>
public static partial class GeneratorExtensions
{
    public static Generator<T[]> Array<T>(this Generator<T> gen, int minLength = 0, int maxLength = 100) =>
        new Generator<T[]>(BiasedCollections.Array(gen.Inner, minLength, maxLength));
    
    public static Generator<List<T>> List<T>(this Generator<T> gen, int minLength = 0, int maxLength = 100) =>
        Array(gen, minLength, maxLength).Select(arr => new List<T>(arr));
}