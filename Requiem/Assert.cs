using TUnit.Assertions.Exceptions;

namespace Prelude.Spec;

public abstract class Assert
{
    protected static void Throw(string? msg1, string? msg2)
    {
#pragma warning disable CS0162 // Unreachable code detected
        string? msg;
        if (msg1 is null && msg2 is null)
            msg = null;
        else if (msg1 is null)
            msg = msg2;
        else if (msg2 is null)
            msg = msg1;
        else
            msg = msg1 + ":\n" + msg2;
        throw new AssertionException(msg);
        return; // To make it possible to step over the exceptions in the debugger
#pragma warning restore CS0162 // Unreachable code detected
    }

    public static void AreEq<T>(IEnumerable<T>? a, IEnumerable<T>? b, string? message = null)
    {
        if (a is null && b is null)
            return;
        if (a is not null && b is null)
            Throw(message, $"\na was {a},\nbut b was null");
        if (a is null && b is not null)
            Throw(message, $"\na was null,\nbut b was {b}");
        SequenceEqual(a!, b!, "a", "b");
    }

    /// <summary>
    /// Test if two IEnumerables are equal, even when they throw exceptions.
    /// To be equal, they must throw the same exceptions at the same index.
    /// </summary>
    private static void SequenceEqual<TSource>(IEnumerable<TSource> a, IEnumerable<TSource> b, string nameA, string nameB, string? message = null)
    {
        if (a is ICollection<TSource> aCol && b is ICollection<TSource> bCol)
        {
            if (aCol.Count != bCol.Count)
            {
                Throw(message, $"Sequences differ in length: {nameA} has length {aCol.Count} while {nameB} has length {bCol.Count}");
            }

            if (aCol is IList<TSource> firstList && bCol is IList<TSource> secondList)
            {
                int count = aCol.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!Equals(firstList[i], secondList[i]))
                    {
                        Throw(message, $"Sequences differ at position {i}: {a} vs {b}");
                    }
                }

                return;
            }
        }

        using (IEnumerator<TSource> e1 = a.GetEnumerator())
        using (IEnumerator<TSource> e2 = b.GetEnumerator())
        {
            int i = 0;
            // Will succeed if MoveNext returns the same value for both iterators,
            // or they throw the same exception
            var hasMore = true;
            while (hasMore)
            {
                AreEq(() => hasMore = e1.MoveNext(), e2.MoveNext, $"a.MoveNext", $"b.MoveNext", message);
                if (hasMore)
                {
                    AreEq(e1.Current, e2.Current, message);
                }
                i++;
            }
        }
    }

    public static void AreEq<T>(T? a, T? b, IEqualityComparer<T> comparer, string? message = null)
    {
        if (!comparer.Equals(a!, b!))
            Throw(message, $"a ({a}),\nis not equal to\nb ({b})");
    }

    public static void AreEq<T>(IEquatable<T>? a, T? b, string? message = null)
        where T : class
    {
        if (a is null && b is null)
            return;
        if (a is not null && b is null)
            Throw(message, $"a was {a},\nbut b was null");
        if (a is null && b is not null)
            Throw(message, $"a was null, but b was {b}");
        if (!a!.Equals(b))
            Throw(message, $"a ({a}),\nis not equal to\nb ({b})");
    }

    public static void AreEq<TA, TB>(TA a, TB b, string? message = null)
        where TA : struct, IEquatable<TB>
        where TB : struct
    {
        if (!a.Equals(b))
            Throw(message, $"a ({a}),\nis not equal to\nb ({b})");
    }

    public static void AreEq(object? a, object? b, string? message = null)
    {
        if (a is not null && b is null)
            Throw(message, $"a was {a},\nbut b was null");
        if (a is null && b is not null)
            Throw(message, $"a was null, but b was {b}");
        if (!Equals(a, b))
            Throw(message, $"a ({a})\nis not equal to\nb ({b})");
    }

    public static void AreEq<T>(Func<T> a, Func<T> b, string? message = null) => AreEq(a, b, "a", "b", message);

    private static void AreEq<T>(Func<T> a, Func<T> b, string nameA, string nameB, string? message = null)
    {
        T? resultA = default;
        T? resultB = default;
        Exception? exA = null, exB = null;

        try
        {
            resultA = a();
        }
        catch (Exception ex)
        {
            exA = ex;
        }
        try
        {
            resultB = b();
        }
        catch (Exception ex)
        {
            exB = ex;
        }

        if (exA is not null && exB is not null)
        {
            if (exA.GetType() != exB.GetType())
                Throw(message, $"{nameA} threw {exA.Message} but {nameB} threw {exB.Message}");
        }
        else if (exA is not null)
        {
            Throw(message, $"{nameA} threw {exA.Message}, but {nameB} returned {resultB}");
        }
        else if (exB is not null)
        {
            Throw(message, $"{nameB} threw {exB.Message}, but {nameA} returned {resultA}");
        }
        else if (!Equals(resultA, resultB))
        {
            Throw(message, $"{nameA} (returned {resultA}) and {nameB} (returned {resultB}) are not equal");
        }
    }

    public static void Throws(Action a, string? message = null)
    {
        bool hasThrown = false;
        try
        {
            a();
        }
        catch
        {
            hasThrown = true;
        }
        if (!hasThrown)
            Throw(message, "Expected exception, but no exception was thrown");
    }

    public static void IsTrue(bool b, string? message = null)
    {
        if (!b)
            Throw(message, "Expected true but was false");
    }

    public static void IsFalse(bool b, string? message = null)
    {
        if (b)
            Throw(message, "Expected false but was true");
    }
}
