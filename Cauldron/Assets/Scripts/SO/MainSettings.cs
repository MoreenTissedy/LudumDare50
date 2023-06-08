using System;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Main settings", menuName = "Main settings", order = 0)]
    public class MainSettings : ScriptableObject
    {
        public Gameplay gameplay;
        public StatusBars statusBars;
        public CatTips catTips;
        
        [Serializable]
        public class Gameplay
        {
             public int defaultStatChange = 10;
             public int defaultMoneyChangeCard = 10;
             public int defaultMoneyChangeEvent = 10;
             public int cardsPerDay = 3;
             public int cardsDealtAtNight = 3;
             public int targetDeckCount = 10;
             public float villagerDelay = 2f;

             [Header("FTUE - Unique cards")]
             [Tooltip("For first N days and for first N rounds only unique cards will be played")]
             public int roundsWithUniqueStartingCards = 2;
             public int daysWithUniqueStartingCards = 3;

        }
        
        [Serializable]
        public class CatTips
        {
            public float SlowTipsDelay = 60;
            public float VisitorCheckDelay = 1;
        }

        [Serializable]
        public class StatusBars
        {
            public int Total = 100;
            public int InitialValue = 50;
            [Tooltip("Percent distance from a bar end")]
            public float InitialThreshold = 30;
            [SerializeField] private float thresholdDecrement = 10;
            [SerializeField] private float minThreshold = 5;

            public int ThresholdDecrement => (int)(thresholdDecrement/100 * Total);
            public int GetMinThreshold => (int)(minThreshold/100 * Total);
            public int GetMaxThreshold => (int)((100f - minThreshold)/100 * Total);
        }
    }
}