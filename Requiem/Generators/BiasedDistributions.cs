using CsCheck;

namespace Requiem.Generators;

/// <summary>
/// Biased generators for statistical distributions.
/// Provides generators with specific probability distributions.
/// </summary>
internal static class BiasedDistributions
{
    /// <summary>
    /// Generates a value with normal (Gaussian) distribution.
    /// </summary>
    public static Gen<bool> Bool(double pTrue = 0.5) =>
        Gen.Double.Unit.Select(u => u <= pTrue);

    /// <summary>
    /// Generates a value with normal (Gaussian) distribution.
    /// </summary>
    public static Gen<double> Normal(double mean = 0, double stdDev = 1) =>
        Gen.Double.Unit.Select(Gen.Double.Unit, (u1, u2) =>
        {
            // Box-Muller transform
            var z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return mean + stdDev * z0;
        });

    /// <summary>
    /// Generates integers with normal distribution.
    /// </summary>
    public static Gen<int> NormalInt(int mean = 0, int stdDev = 10) =>
        Normal(mean, stdDev).Select(d => (int)Math.Round(d));

    /// <summary>
    /// Generates values that tend to stay close to previous values (random walk).
    /// </summary>
    public static Gen<int[]> RandomWalk(int start, int steps, int maxStep = 5) =>
        Gen.Int[-maxStep, maxStep].Array[steps].Select(deltas =>
        {
            var result = new int[steps + 1];
            result[0] = start;
            for (int i = 0; i < steps; i++)
            {
                result[i + 1] = result[i] + deltas[i];
            }
            return result;
        });
}