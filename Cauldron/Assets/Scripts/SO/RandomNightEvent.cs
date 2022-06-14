using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Random event", menuName = "Night event/Random", order = 8)]
    public class RandomNightEvent : ConditionalEvent
    {
        [Header("Random parameters")] 
        public int minDay;
        [Range(0, 100)] 
        public int probability;

        public override bool Valid(GameState game)
        {
            if (game.currentDay < minDay)
            {
                return false;
            }

            int random = Random.Range(0, 100);
            if (random <= probability)
            {
                return true;
            }
            return false;
            
        }
    }
}