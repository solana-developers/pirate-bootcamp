using System;
using System.Collections.Generic;

namespace SolPlay.Orca
{
   [Serializable]
   public class OrcaApiTokenData
   {
      public List<Token> tokens;
   }

   [Serializable]
    public class OrcaApiPoolsData
    {
        public List<whirlpool> whirlpools;
    }

    [Serializable]
    public class Token
    {
       public string mint;
       public string symbol;
       public string name;
       public int decimals;
       public string logoURI;
       public string coingeckoId;
       public bool whitelisted;
       public bool poolToken;
    }

    [Serializable]
    public class DayWeekMonth
    {
       public double day;
       public double week;
       public double month;
    }
    
    [Serializable]
    public class PriceRange
    {
       public MinMax day;
       public MinMax week;
       public MinMax month;
    }

    [Serializable]
    public class MinMax
    {
       public double min;
       public double max;
    }

    [Serializable]
    public class whirlpool
    {
       public string address;
       public Token tokenA;
       public Token tokenB;
       public bool whitelisted;
       public int tickSpacing;
       public double price;
       public double lpFeeRate;
       public double protocolFeeRate;
       public string whirlpoolsConfig;
       public long modifiedTimeMs;
       public double tvl;
       public DayWeekMonth volume;
       public DayWeekMonth volumeDenominatedA;
       public DayWeekMonth volumeDenominatedB;
       public PriceRange priceRange;
       public DayWeekMonth feeApr;
       public DayWeekMonth reward0Apr;
       public DayWeekMonth reward1Apr;
       public DayWeekMonth reward2Apr;
       public DayWeekMonth totalApr;
    }
}