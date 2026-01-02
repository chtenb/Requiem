using CsCheck;

namespace Requiem;

/// <summary>
/// Represents a generator that produces random values of type T.
/// This is Requiem's abstraction over the underlying property testing framework.
/// </summary>
public sealed class Generator<T>
{
    internal Generator(Gen<T> inner) => Inner = inner;

    /// <summary>
    /// Internal access to the underlying CsCheck generator.
    /// </summary>
    internal Gen<T> Inner { get; }

    /// <summary>
    /// Generate a single value (convenience method).
    /// </summary>
    public T Single() => Inner.Single();

    /// <summary>
    /// Check that a property holds for all generated values.
    /// Automatically shrinks failing cases to find minimal counterexamples.
    /// </summary>
    /// <param name="gen">Generator for test values.</param>
    /// <param name="property">Property to verify.</param>
    /// <param name="iter">Number of test iterations (default: 100).</param>
    /// <param name="time">Optional time limit in seconds.</param>
    /// <param name="seed">Optional seed for reproducible tests.</param>
    /// <param name="print">Optional custom print function for values.</param>
    public void Check(
        Action<T> assert,
        long iter = 1000,
        int time = -1,
        int threads = -1,
        string? seed = null,
        Func<T, string>? print = null)
    {
        Inner.Sample(assert, iter: iter, time: time, seed: seed, print: print, threads: threads);
    }

}

/// <summary>
/// Extension methods for Generator of tuples to enable convenient property checking.
/// </summary>
public static class GeneratorTupleExtensions
{
    /// <summary>
    /// Check that a property holds for all pairs of generated values.
    /// </summary>
    public static void Check<T1, T2>(
        this Generator<(T1, T2)> gen,
        Action<T1, T2> assert,
        long iter = 1000,
        int time = -1,
        int threads = -1,
        string? seed = null,
        Func<T1, T2, string>? print = null)
    {
        gen.Inner.Sample(
            t => assert(t.Item1, t.Item2),
            iter: iter, time: time, seed: seed, threads: threads,
            print: print != null ? (t => print(t.Item1, t.Item2)) : null);
    }

    /// <summary>
    /// Check that a property holds for all triples of generated values.
    /// </summary>
    public static void Check<T1, T2, T3>(
        this Generator<(T1, T2, T3)> gen,
        Action<T1, T2, T3> assert,
        long iter = 1000,
        int time = -1,
        int threads = -1,
        string? seed = null,
        Func<T1, T2, T3, string>? print = null)
    {
        gen.Inner.Sample(
            t => assert(t.Item1, t.Item2, t.Item3),
            iter: iter, time: time, seed: seed, threads: threads,
            print: print != null ? (t => print(t.Item1, t.Item2, t.Item3)) : null);
    }

    /// <summary>
    /// Check that a property holds for all 4-tuples of generated values.
    /// </summary>
    public static void Check<T1, T2, T3, T4>(
        this Generator<(T1, T2, T3, T4)> gen,
        Action<T1, T2, T3, T4> assert,
        long iter = 1000,
        int time = -1,
        int threads = -1,
        string? seed = null,
        Func<T1, T2, T3, T4, string>? print = null)
    {
        gen.Inner.Sample(
            t => assert(t.Item1, t.Item2, t.Item3, t.Item4),
            iter: iter, time: time, seed: seed, threads: threads,
            print: print != null ? (t => print(t.Item1, t.Item2, t.Item3, t.Item4)) : null);
    }

    /// <summary>
    /// Check that a property holds for all 5-tuples of generated values.
    /// </summary>
    public static void Check<T1, T2, T3, T4, T5>(
        this Generator<(T1, T2, T3, T4, T5)> gen,
        Action<T1, T2, T3, T4, T5> assert,
        long iter = 1000,
        int time = -1,
        int threads = -1,
        string? seed = null,
        Func<T1, T2, T3, T4, T5, string>? print = null)
    {
        gen.Inner.Sample(
            t => assert(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5),
            iter: iter, time: time, seed: seed, threads: threads,
            print: print != null ? (t => print(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5)) : null);
    }
}