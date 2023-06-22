using UnityEngine;

namespace SolPlay.DeeplinksNftExample.Scripts.OrcaWhirlPool
{
    public class TickUtils
    {
        public const int TICK_ARRAY_SIZE = 88;
        public const int MIN_TICK_INDEX = -443636;
        public const int MAX_TICK_INDEX = 443636;

        public static int GetStartTickIndex(int tickIndex, int tickSpacing, int offset)
        {
            var realIndex = Mathf.FloorToInt((float) tickIndex / (float)tickSpacing / (float)TICK_ARRAY_SIZE);
            var startTickIndex = (realIndex + offset) * tickSpacing * TICK_ARRAY_SIZE;
            var ticksInArray = TICK_ARRAY_SIZE * tickSpacing;
            var minTickIndex = MIN_TICK_INDEX - ((MIN_TICK_INDEX % ticksInArray) + ticksInArray);
            if (startTickIndex <= minTickIndex)
            {
                Debug.LogError($"Tick index too small {startTickIndex}");
            }
            if (startTickIndex >= MAX_TICK_INDEX)
            {
                Debug.LogError($"Tick index too big {startTickIndex}");
            }
            return startTickIndex;
        }
    }
}