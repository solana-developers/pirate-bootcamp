using System.Numerics;
using NUnit.Framework;
using SolPlay.Orca.OrcaWhirlPool;
using Assert = UnityEngine.Assertions.Assert;

public class OrcaMathTests
{
    [Test]
    public void SqrtPriceToPrice_Case_ORCA()
    {
        BigInteger bigInt = BigInteger.Parse("17045354231584594377");
        var result = PriceUtils.GetPriceFromSqrtPrice(bigInt);
        double expected = 0.85383237312353633;
        Assert.AreEqual(result, expected);
    }
}
