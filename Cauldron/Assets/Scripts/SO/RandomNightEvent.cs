using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Random event", menuName = "Night event/Random", order = 8)]
    public class RandomNightEvent : ConditionalEvent
    {
        [Header("Random parameters")] 
        public int minDay;
        public int minRound;
        public string requiredStoryTag;

        public override bool Valid(GameDataHandler game)
        {
            if (game.currentDay < minDay)
            {
                return false;
            }
            if (game.currentRound < minRound)
            {
                return false;
            }
            if (!StoryTagHelper.Check(requiredStoryTag, game))
            {
                return false;
            }
            return true;
        }
    }
}