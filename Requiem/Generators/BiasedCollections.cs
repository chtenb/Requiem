using CsCheck;

namespace Requiem.Generators;

internal static class BiasedCollections
{
    public static Gen<T[]> UniqueArray<T>(Gen<T> elementGen, int minLength, int maxLength)
    {
        return Array(elementGen, minLength, maxLength).Select(arr => arr.Distinct().ToArray());
    }
    
    public static Gen<T[]> Array<T>(Gen<T> elementGen, int minLength, int maxLength)
    {
        var generators = new List<(int, Gen<T[]>)>();

        // Empty array only if minLength allows it
        if (minLength == 0)
            generators.Add((10, Gen.Const(System.Array.Empty<T>())));

        // Single element only if minLength allows it
        if (minLength <= 1)
            generators.Add((10, elementGen.Select(x => new[] { x })));

        // Two elements only if minLength allows it
        if (minLength <= 2)
            generators.Add((10, elementGen.Select(x => new[] { x, x })));

        // Three elements only if minLength allows it
        if (minLength <= 3)
            generators.Add((10, elementGen.Select(x => new[] { x, x, x })));

        // All same elements
        generators.Add((10, AllSameElements(elementGen, minLength, maxLength)));

        // Very large array
        generators.Add((10, VeryLargeArray(elementGen, minLength, maxLength)));

        // Array with dimensional correlations (same elements at some positions)
        generators.Add((10, ArrayWithDimensionalCorrelations(elementGen, minLength, maxLength)));

        // Random array within bounds
        generators.Add((10, elementGen.Array[minLength, maxLength]));

        return Gen.Frequency([.. generators]);
    }

    private static Gen<T[]> AllSameElements<T>(Gen<T> elementGen, int minLength, int maxLength) =>
        elementGen.Select(Gen.Int[minLength, maxLength], (element, count) =>
        {
            var result = new T[count];
            for (int i = 0; i < count; i++)
                result[i] = element;
            return result;
        });

    private static Gen<T[]> VeryLargeArray<T>(Gen<T> elementGen, int minLength, int maxLength)
    {
        var sizes = new List<int>();

        if (1000 <= minLength)
            sizes.Add(1000);
        if (10000 <= minLength)
            sizes.Add(10000);
        if (maxLength <= minLength)
            sizes.Add(maxLength);

        // If no valid sizes, use minLength
        if (sizes.Count == 0)
            sizes.Add(minLength);

        return Gen.OneOfConst(sizes.ToArray())
            .SelectMany(size => elementGen.Array[size, size]);
    }

    private static Gen<T[]> ArrayWithDimensionalCorrelations<T>(Gen<T> elementGen, int minLength, int maxLength) =>
        Gen.Int[minLength, maxLength]
            .Select(Gen.Double[0.2, 0.6], (size, correlationRate) =>
            {
                // Generate a pool of elements to reuse
                var poolSize = (int)(size * (1 - correlationRate));
                if (poolSize < 1) poolSize = 1;
                return elementGen.Array[poolSize, poolSize].Select(pool =>
                {
                    var result = new T[size];
                    var random = new Random();

                    for (int i = 0; i < size; i++)
                    {
                        // Pick a random element from the pool to create correlations
                        result[i] = pool[random.Next(pool.Length)];
                    }

                    return result;
                });
            })
            .SelectMany(x => x);
}