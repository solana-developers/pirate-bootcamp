using System;
using System.Numerics;

namespace SolPlay.Orca.OrcaWhirlPool
{
    public static class PriceUtils
    {
        public static double GetPriceFromSqrtPrice(BigInteger sqrtPrice)
        {
            var d = Double.Parse(sqrtPrice.ToString());
            var fromX64 = Math.Pow(d * Math.Pow(2, -64), 2);
            return fromX64;
        }
    }
}