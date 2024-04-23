using UnityEngine;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "CovenNightEventProvider", menuName = "CovenEventProvider", order = 0)]
    public class CovenNightEventProvider : ScriptableObject
    {
        public NightEvent[] HighFame;
        public NightEvent[] LowFame;
        public NightEvent[] HighFear;
        public NightEvent[] LowFear;

        public NightEvent GetRandom(Statustype status, bool high)
        {
            switch (status)
            {
                case Statustype.Fear:
                    return high ? GetRandom(HighFear) : GetRandom(LowFear);
                case Statustype.Fame:
                    return high ? GetRandom(HighFame) : GetRandom(LowFame);
            }
            Debug.LogError("Trying to get a coven night card with wrong status type: "+status);
            return null;
        }

        private NightEvent GetRandom(NightEvent[] collection)
        {
            return collection[Random.Range(0, collection.Length)];
        }
    }
}