using System.Collections.Generic;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Event Provider", menuName = "Event Provider", order = 9)]
    public class NightEventProvider : ScriptableObject
    {
        public List<ConditionalEvent> conditionalEvents;
        public List<NightEvent> storyEvents;

        public NightEvent CheckConditions(GameState game)
        {
            //conditionalEvents - take 1
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

        public NightEvent[] GetEvents(GameState game)
        {
            List<NightEvent> returnEvents = new List<NightEvent>(storyEvents.Count+1);
            returnEvents.AddRange(storyEvents);
            NightEvent conditionalEvent = CheckConditions(game);
            if (!(conditionalEvent is null))
            {
                returnEvents.Add(conditionalEvent);
            }
            storyEvents.Clear();
            return returnEvents.ToArray();
        }
    }
}