namespace Requiem.Test;

public class Test
{
    [Test]
    public void TestParameterizedGenerators()
    {
        // Test Range generators with randomized parameters
        // The main goal is to ensure generators don't throw exceptions with various parameter combinations
        Gens.Range(0, 1000).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                // Test integer range - just ensure it doesn't throw
                var intGen = Gens.Range(min, max);
                _ = intGen.Single();
                
                // Test long range
                var longGen = Gens.Range((long)min, (long)max);
                _ = longGen.Single();
                
                // Test double range
                var doubleGen = Gens.Range((double)min, (double)max);
                _ = doubleGen.Single();
                
                // Test float range
                var floatGen = Gens.Range((float)min, (float)max);
                _ = floatGen.Single();
                
                // Test decimal range
                var decimalGen = Gens.Range((decimal)min, (decimal)max);
                _ = decimalGen.Single();
            }, iter: 10000);
        
        // Test String generator with randomized length parameters
        Gens.Range(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                var stringGen = Gens.String(minLen, maxLen);
                _ = stringGen.Single();
            }, iter: 10000);
        
        // Test Array generator with randomized length parameters
        Gens.Range(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                var arrayGen = Gens.Int.Array(minLen, maxLen);
                _ = arrayGen.Single();
            }, iter: 10000);
        
        // Test List generator with randomized length parameters
        Gens.Range(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                var listGen = Gens.Int.List(minLen, maxLen);
                _ = listGen.Single();
            }, iter: 10000);
    }
}
