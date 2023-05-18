using System;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Main settings", menuName = "Main settings", order = 0)]
    public class MainSettings : ScriptableObject
    {
        public Gameplay gameplay;
        public StatusBars statusBars;
        
        [Serializable]
        public class Gameplay
        {
             public int defaultStatChange = 10;
             public int defaultMoneyChangeCard = 10;
             public int defaultMoneyChangeEvent = 10;
             public int cardsPerDay = 3;
             public int cardsDealtAtNight = 3;
             public float villagerDelay = 2f;

             public float nightStartDelay = 3.5f;

             public int roundsWithUniqueStartingCards = 2; //Первые N-1 кругов необходимо сохранять все карты
             public int daysWithUniqueStartingCards = 3; // выпавшие в первые три X дня игры

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