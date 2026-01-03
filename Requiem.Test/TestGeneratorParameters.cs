using CsCheck;

namespace Requiem.Test;

public class TestGeneratorParameters
{

    [Test]
    public void Repro()
    {
        _ = CsCheck.Gen.Double[10e14, 10e15].Single();
    }

    [Test]
    public void Int()
    {
        Gens.Between(0, 1000).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Gens.Between(min, max);
                _ = gen.Single();
            }, iter: 10000);
    }

    [Test]
    public void Long()
    {
        Gens.Between(0L, 1000L).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Gens.Between(min, max);
                _ = gen.Single();
            }, iter: 10000);
    }

    [Test]
    public void Double()
    {
        Gens.Between(0.0, 1000.0).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Gens.Between(min, max);
                _ = gen.Single();
            }, iter: 10000);
    }

    [Test]
    public void Float()
    {
        Gens.Between(0.0f, 1000.0f).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Gens.Between(min, max);
                _ = gen.Single();
            }, iter: 10000);
    }

    [Test]
    public void Decimal()
    {
        Gens.Between(0m, 1000m).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Gens.Between(min, max);
                _ = gen.Single();
            }, iter: 10000);
    }

    [Test]
    public void String()
    {
        Gens.Between(0, 1000).Tuple()
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
        Gens.Between(0, 1000).Tuple()
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
        Gens.Between(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                var listGen = Gens.Int.List(minLen, maxLen);
                _ = listGen.Single();
            }, iter: 10000);
    }
}

