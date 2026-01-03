using CsCheck;

namespace Requiem.Generators;

internal static class BiasedDistributions
{
    public static Gen<bool> Bool(double pTrue = 0.5) =>
        Gen.Double.Unit.Select(u => u <= pTrue);

    public static Gen<double> Normal(double mean = 0, double stdDev = 1) =>
        Gen.Double.Unit.Select(Gen.Double.Unit, (u1, u2) =>
        {
            // Box-Muller transform
            var z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return mean + stdDev * z0;
        });

    public static Gen<int> NormalInt(int mean = 0, int stdDev = 10) =>
        Normal(mean, stdDev).Select(d => (int)Math.Round(d));

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