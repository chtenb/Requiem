namespace Requiem.Tutorial;

/// <summary>
/// Tutorial 05: Advanced Examples - Why Biased Generators Matter
///
/// This tutorial demonstrates real-world testing scenarios where biased generators
/// excel by generating correlated edge cases across multiple dimensions.
///
/// KEY INSIGHT: Uniform random generation struggles with dimensional correlation.
/// For example, generating two equal objects requires the generator to produce
/// identical edge cases in both dimensions - essentially impossible with uniform random.
/// Biased generators solve this by correlating edge cases, making bugs discoverable.
/// </summary>
public class AdvancedExamples
{
    /// <summary>
    /// Testing Equality Laws with Dimensional Correlation
    ///
    /// This example tests that custom equality implementations satisfy fundamental laws.
    /// The challenge: many bugs only appear when BOTH objects in a tuple are edge cases.
    ///
    /// Why uniform random fails:
    /// - P(both empty arrays) ≈ very small, decreases exponentially with possible sizes
    /// - P(both at DateTime.MinValue) ≈ 1 in 10^18
    /// - P(both objects equal AND edge cases) ≈ essentially zero
    ///
    /// Why biased generators succeed:
    /// - Correlates edge cases: generates (empty, empty), (MinValue, MinValue), etc.
    /// - Tests reflexivity with actual edge case values
    /// - Finds hash code bugs that only appear with equal edge cases
    /// </summary>
    public void TestEqualityLaws<T>(Generator<T> gen)
        where T : IEquatable<T>
    {
        gen.Tuple().Check((a, b) =>
        {
            // Reflexivity: a.Equals(a) must always be true
            // Biased generator tests this with edge cases like TimeInterval.Never, TimeInterval.Always
            Assert.IsTrue(a.Equals(a), $"Reflexivity failed for {a}");

            // Symmetry: if a.Equals(b) then b.Equals(a)
            // Requires generating EQUAL objects, which needs dimensional correlation
            Assert.AreEq(a.Equals(b), b.Equals(a),
                $"Symmetry failed: {a}.Equals({b}) = {a.Equals(b)}, but {b}.Equals({a}) = {b.Equals(a)}");

            // Consistency with object.Equals operator
            Assert.AreEq(a.Equals(b), object.Equals(a, b),
                $"Equals and == operator inconsistent for {a} and {b}");

            // Hash code contract: equal objects must have equal hash codes
            // This is the critical test that needs dimensional correlation:
            // We need to generate (a, b) where a.Equals(b) is true
            // Uniform random almost never generates equal edge cases
            if (a.Equals(b))
            {
                var hashA = a.GetHashCode();
                var hashB = b.GetHashCode();
                Assert.AreEq(hashA, hashB,
                    $"Hash code contract violated: {a} and {b} are equal but have different hash codes: {hashA} vs {hashB}");
            }
        });
    }
}
