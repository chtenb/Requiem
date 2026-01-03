using CsCheck;

namespace Requiem;

/// <summary>
/// Tuple extension methods for Generator<T> (Tuple, Zip).
/// </summary>
public static partial class GeneratorExtensions
{
    /// <summary>
    /// Generate a tuple of two values.
    /// </summary>
    public static Generator<(T, T)> Tuple<T>(this Generator<T> gen) =>
        gen.Array(2, 2).Map(arr => (arr[0], arr[1]));

    /// <summary>
    /// Generate a tuple of three values.
    /// </summary>
    public static Generator<(T, T, T)> Tuple3<T>(this Generator<T> gen) =>
        gen.Array(3, 3).Map(arr => (arr[0], arr[1], arr[2]));

    /// <summary>
    /// Generate a tuple of four values.
    /// </summary>
    public static Generator<(T, T, T, T)> Tuple4<T>(this Generator<T> gen) =>
        gen.Array(4, 4).Map(arr => (arr[0], arr[1], arr[2], arr[3]));

    /// <summary>
    /// Generate a tuple combining two different generator types.
    /// </summary>
    public static Generator<(T1, T2)> Zip<T1, T2>(
        this Generator<T1> gen1,
        Generator<T2> gen2) =>
        new(Gen.Select(gen1.Inner, gen2.Inner));

    /// <summary>
    /// Generate a tuple combining three different generator types.
    /// </summary>
    public static Generator<(T1, T2, T3)> Zip<T1, T2, T3>(
        this Generator<T1> gen1,
        Generator<T2> gen2,
        Generator<T3> gen3) =>
        new(Gen.Select(gen1.Inner, gen2.Inner, gen3.Inner));
}