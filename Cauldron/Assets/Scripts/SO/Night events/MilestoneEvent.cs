using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Milestone event", menuName = "Night event/Milestone", order = 9)]
    public class MilestoneEvent: ConditionalEvent
    {
        public int fixedDayRequirement = 1;
        public string requiredStoryTag;
        
        private bool everWasValid;
        private bool wasResolved;
        
        public override bool Valid(GameDataHandler game)
        {
            if (game.currentDay == fixedDayRequirement && StoryTagHelper.Check(requiredStoryTag, game))
            {
                everWasValid = true;
                return true;
            };
            return everWasValid && !wasResolved;
        }

        public override void OnResolve()
        {
            base.OnResolve();
            wasResolved = true;
        }
    }
}