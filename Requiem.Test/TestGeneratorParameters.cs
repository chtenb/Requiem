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
        Generate.Between(0, 1000).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Generate.Between(min, max);
                _ = gen.Next();
            }, iter: 10000);
    }

    [Test]
    public void Long()
    {
        Generate.Between(0L, 1000L).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Generate.Between(min, max);
                _ = gen.Next();
            }, iter: 10000);
    }

    [Test]
    public void Double()
    {
        Generate.Between(0.0, 1000.0).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Generate.Between(min, max);
                _ = gen.Next();
            }, iter: 10000);
    }

    [Test]
    public void Float()
    {
        Generate.Between(0.0f, 1000.0f).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Generate.Between(min, max);
                _ = gen.Next();
            }, iter: 10000);
    }

    [Test]
    public void Decimal()
    {
        Generate.Between(0m, 1000m).Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                var gen = Generate.Between(min, max);
                _ = gen.Next();
            }, iter: 10000);
    }

    [Test]
    public void String()
    {
        Generate.Between(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                var stringGen = Generate.String(minLen, maxLen);
                _ = stringGen.Next();
            }, iter: 10000);
    }

    [Test]
    public void Array()
    {
        Generate.Between(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                var arrayGen = Generate.Int.Array(minLen, maxLen);
                _ = arrayGen.Next();
            }, iter: 10000);
    }

    [Test]
    public void List()
    {
        Generate.Between(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                var listGen = Generate.Int.List(minLen, maxLen);
                _ = listGen.Next();
            }, iter: 10000);
    }
}

