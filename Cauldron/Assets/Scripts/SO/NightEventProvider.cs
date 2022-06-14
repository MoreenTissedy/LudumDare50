using System.Collections.Generic;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Event Provider", menuName = "Event Provider", order = 9)]
    public class NightEventProvider : ScriptableObject
    {
        //conditionalEvents - take 1
        public List<ConditionalEvent> conditionalEvents;
        public List<NightEvent> storyEvents;

        NightEvent CheckConditions(GameState game)
        {
            ConditionalEvent validEvent = null;
            bool foundValid = false;
            foreach (ConditionalEvent check in conditionalEvents)
            {
                if (check.Valid(game))
                {
                    foundValid = true;
                    validEvent = check;
                    break;
                }
            }
            if (foundValid)
            {
                conditionalEvents.Remove(validEvent);
            }

            return validEvent;
        }
    }
}