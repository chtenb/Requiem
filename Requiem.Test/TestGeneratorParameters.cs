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
        Generate.Int().Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                _ = Generate.Int(min, max).Next();
            }, iter: 10000);
    }

    [Test]
    public void Long()
    {
        Generate.Long().Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                _ = Generate.Long(min, max).Next();
            }, iter: 10000);
    }

    [Test]
    public void Double()
    {
        Generate.Double().Tuple()
            .Filter((min, max) => min <= max)
            .Check((min, max) =>
            {
                _ = Generate.Double(min, max).Next();
            }, iter: 10000);
    }

    [Test]
    public void String()
    {
        Generate.Int(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                _ = Generate.String(minLen, maxLen).Next();
            }, iter: 10000);
    }

    [Test]
    public void Array()
    {
        Generate.Int(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                _ = Generate.Int().Array(minLen, maxLen).Next();
            }, iter: 10000);
    }

    [Test]
    public void List()
    {
        Generate.Int(0, 1000).Tuple()
            .Filter((minLen, maxLen) => minLen <= maxLen)
            .Check((minLen, maxLen) =>
            {
                _ = Generate.Int().List(minLen, maxLen).Next();
            }, iter: 10000);
    }
}

