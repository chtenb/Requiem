namespace Requiem.Test;

public class TestGeneratorParameters
{
    [Test]
    public void Int()
    {
        Gens.Range(0, 1000).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Gens.Range(min, max);
                _ = gen.Single();
            }, iter: 10000);
    }

    [Test]
    public void Long()
    {
        Gens.Range(0L, 1000L).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Gens.Range(min, max);
                _ = gen.Single();
            }, iter: 10000);
    }

    [Test]
    public void Double()
    {
        Gens.Range(0.0, 1000.0).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Gens.Range(min, max);
                _ = gen.Single();
            }, iter: 10000);
    }

    [Test]
    public void Float()
    {
        Gens.Range(0.0f, 1000.0f).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Gens.Range(min, max);
                _ = gen.Single();
            }, iter: 10000);
    }

    [Test]
    public void Decimal()
    {
        Gens.Range(0m, 1000m).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Gens.Range(min, max);
                _ = gen.Single();
            }, iter: 10000);
    }

    [Test]
    public void String()
    {
        Gens.Range(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                var stringGen = Gens.String(minLen, maxLen);
                _ = stringGen.Single();
            }, iter: 10000);
    }

    [Test]
    public void Array()
    {
        Gens.Range(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                var arrayGen = Gens.Int.Array(minLen, maxLen);
                _ = arrayGen.Single();
            }, iter: 10000);
    }

    [Test]
    public void List()
    {
        Gens.Range(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                var listGen = Gens.Int.List(minLen, maxLen);
                _ = listGen.Single();
            }, iter: 10000);
    }
}

