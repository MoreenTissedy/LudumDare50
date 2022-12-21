using System.Collections.Generic;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Event Provider", menuName = "Event Provider", order = 9)]
    public class NightEventProvider : ScriptableObject
    {
        public NightEvent intro;
        public List<ConditionalEvent> conditionalEvents;
        [Header("Readonly")]
        public List<NightEvent> storyEvents;
        public List<ConditionalEvent> inGameConditionals;

        public void Init()
        {
            inGameConditionals = new List<ConditionalEvent>(conditionalEvents.Count);
            inGameConditionals.AddRange(conditionalEvents);
            storyEvents.Clear();
        }

        private NightEvent CheckConditions(GameData game)
        {
            //conditionalEvents - take 1
            ConditionalEvent validEvent = null;
            bool foundValid = false;
            foreach (ConditionalEvent check in inGameConditionals)
            {
                if (check.Valid(game))
                {
                    foundValid = true;
                    validEvent = check;
                    break;
                }
            }
            if (foundValid && validEvent.Repeat == false)
            {
                inGameConditionals.Remove(validEvent);
            }

            return validEvent;
        }

        public NightEvent[] GetEvents(GameData game)
        {
            if (!PlayerPrefs.HasKey("FirstNight"))
            {
                PlayerPrefs.SetInt("FirstNight", 1);
                return new NightEvent[] {intro};
            }
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