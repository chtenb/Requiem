using CsCheck;

namespace Requiem;

/// <summary>
/// Transformation extension methods for Generator<T> (Select, Where, Chain).
/// </summary>
public static partial class GeneratorExtensions
{
    /// <summary>
    /// Transform the generated values using a selector function.
    /// Pass-through to CsCheck's Select to preserve shrinking.
    /// </summary>
    public static Generator<TResult> Select<T, TResult>(
        this Generator<T> gen,
        Func<T, TResult> selector) =>
        new(gen.Inner.Select(selector));

    /// <summary>
    /// Transform generated tuple values using a selector function with unpacked parameters.
    /// </summary>
    public static Generator<TResult> Select<T1, T2, TResult>(
        this Generator<(T1, T2)> gen,
        Func<T1, T2, TResult> selector) =>
        new(gen.Inner.Select(t => selector(t.Item1, t.Item2)));

    /// <summary>
    /// Transform generated tuple values using a selector function with unpacked parameters.
    /// </summary>
    public static Generator<TResult> Select<T1, T2, T3, TResult>(
        this Generator<(T1, T2, T3)> gen,
        Func<T1, T2, T3, TResult> selector) =>
        new(gen.Inner.Select(t => selector(t.Item1, t.Item2, t.Item3)));

    /// <summary>
    /// Transform generated tuple values using a selector function with unpacked parameters.
    /// </summary>
    public static Generator<TResult> Select<T1, T2, T3, T4, TResult>(
        this Generator<(T1, T2, T3, T4)> gen,
        Func<T1, T2, T3, T4, TResult> selector) =>
        new(gen.Inner.Select(t => selector(t.Item1, t.Item2, t.Item3, t.Item4)));

    /// <summary>
    /// Transform generated tuple values using a selector function with unpacked parameters.
    /// </summary>
    public static Generator<TResult> Select<T1, T2, T3, T4, T5, TResult>(
        this Generator<(T1, T2, T3, T4, T5)> gen,
        Func<T1, T2, T3, T4, T5, TResult> selector) =>
        new(gen.Inner.Select(t => selector(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5)));

    /// <summary>
    /// Filter generated values to only include those matching the predicate.
    /// Pass-through to CsCheck's Where to preserve shrinking.
    /// </summary>
    public static Generator<T> Where<T>(
        this Generator<T> gen,
        Func<T, bool> predicate) =>
        new(gen.Inner.Where(predicate));

    /// <summary>
    /// Filter generated tuple values using a predicate with unpacked parameters.
    /// </summary>
    public static Generator<(T1, T2)> Where<T1, T2>(
        this Generator<(T1, T2)> gen,
        Func<T1, T2, bool> predicate) =>
        new(gen.Inner.Where(t => predicate(t.Item1, t.Item2)));

    /// <summary>
    /// Filter generated tuple values using a predicate with unpacked parameters.
    /// </summary>
    public static Generator<(T1, T2, T3)> Where<T1, T2, T3>(
        this Generator<(T1, T2, T3)> gen,
        Func<T1, T2, T3, bool> predicate) =>
        new(gen.Inner.Where(t => predicate(t.Item1, t.Item2, t.Item3)));

    /// <summary>
    /// Filter generated tuple values using a predicate with unpacked parameters.
    /// </summary>
    public static Generator<(T1, T2, T3, T4)> Where<T1, T2, T3, T4>(
        this Generator<(T1, T2, T3, T4)> gen,
        Func<T1, T2, T3, T4, bool> predicate) =>
        new(gen.Inner.Where(t => predicate(t.Item1, t.Item2, t.Item3, t.Item4)));

    /// <summary>
    /// Filter generated tuple values using a predicate with unpacked parameters.
    /// </summary>
    public static Generator<(T1, T2, T3, T4, T5)> Where<T1, T2, T3, T4, T5>(
        this Generator<(T1, T2, T3, T4, T5)> gen,
        Func<T1, T2, T3, T4, T5, bool> predicate) =>
        new(gen.Inner.Where(t => predicate(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5)));

    /// <summary>
    /// Chain this generator with another, using the value from this generator.
    /// </summary>
    public static Generator<T> Chain<T>(
        this Generator<T> gen,
        Func<T, Generator<T>> next) =>
        new(gen.Inner.SelectMany(x => next(x).Inner));

    /// <summary>
    /// Chain this tuple generator with another, using unpacked values from this generator.
    /// </summary>
    public static Generator<(T1, T2)> Chain<T1, T2>(
        this Generator<(T1, T2)> gen,
        Func<T1, T2, Generator<(T1, T2)>> next) =>
        new(gen.Inner.SelectMany(t => next(t.Item1, t.Item2).Inner));

    /// <summary>
    /// Chain this tuple generator with another, using unpacked values from this generator.
    /// </summary>
    public static Generator<(T1, T2, T3)> Chain<T1, T2, T3>(
        this Generator<(T1, T2, T3)> gen,
        Func<T1, T2, T3, Generator<(T1, T2, T3)>> next) =>
        new(gen.Inner.SelectMany(t => next(t.Item1, t.Item2, t.Item3).Inner));

    /// <summary>
    /// Chain this tuple generator with another, using unpacked values from this generator.
    /// </summary>
    public static Generator<(T1, T2, T3, T4)> Chain<T1, T2, T3, T4>(
        this Generator<(T1, T2, T3, T4)> gen,
        Func<T1, T2, T3, T4, Generator<(T1, T2, T3, T4)>> next) =>
        new(gen.Inner.SelectMany(t => next(t.Item1, t.Item2, t.Item3, t.Item4).Inner));

    /// <summary>
    /// Chain this tuple generator with another, using unpacked values from this generator.
    /// </summary>
    public static Generator<(T1, T2, T3, T4, T5)> Chain<T1, T2, T3, T4, T5>(
        this Generator<(T1, T2, T3, T4, T5)> gen,
        Func<T1, T2, T3, T4, T5, Generator<(T1, T2, T3, T4, T5)>> next) =>
        new(gen.Inner.SelectMany(t => next(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5).Inner));
}