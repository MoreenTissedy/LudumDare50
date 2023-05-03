using System;
using System.Collections.Generic;
using System.Linq;
using Save;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Event Provider", menuName = "Event Provider", order = 9)]
    [Serializable]
    public class NightEventProvider : ScriptableObject, IDataPersistence
    {
        private const int _EVENT_COOLDOWN_ = 3;
        private const int _EVENT_COUNT_FOR_RANDOM_EVENT_ = 1;

        [Serializable]
        public class CooldownEvent
        {
            public int Days;
            public ConditionalEvent Event;

            public CooldownEvent(ConditionalEvent conditionalEvent)
            {
                Days = _EVENT_COOLDOWN_;
                Event = conditionalEvent;
            } 
            public CooldownEvent(ConditionalEvent conditionalEvent, int days)
            {
                Days = days;
                Event = conditionalEvent;
            } 
        }

        public NightEvent intro;
        public List<ConditionalEvent> conditionalEvents;
        public List<RandomNightEvent> randomEvents;
        [Header("Readonly")]
        public List<NightEvent> storyEvents;
        public List<ConditionalEvent> inGameConditionals;
        public List<RandomNightEvent> inGameRandoms;
        public List<CooldownEvent> eventsOnCooldown;
        private SODictionary soDictionary;

        public void Init(DataPersistenceManager dataPersistenceManager, SODictionary dictionary)
        {
            eventsOnCooldown = new List<CooldownEvent>(5);
            inGameConditionals = new List<ConditionalEvent>(conditionalEvents.Count);
            inGameConditionals.AddRange(conditionalEvents);
            inGameRandoms = new List<RandomNightEvent>(randomEvents.Count);
            inGameRandoms.AddRange(randomEvents);
            soDictionary = dictionary;
            dataPersistenceManager.AddToDataPersistenceObjList(this);
        }

        private NightEvent GetRandomEvent(int day)
        {
            var validEvents = new List<RandomNightEvent>(inGameRandoms.Count);
            foreach (var randomNightEvent in inGameRandoms)
            {
                if (randomNightEvent.minDay <= day)
                {
                    validEvents.Add(randomNightEvent);
                }
            }
            var result = validEvents[Random.Range(0, validEvents.Count)];
            if (result.repeat)
            {
                eventsOnCooldown.Add(new CooldownEvent(result));
            }
            inGameRandoms.Remove(result);
            return result;
        }

        private NightEvent GetConditionalEvent(GameDataHandler game)
        {
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

            if (foundValid)
            {
                if (validEvent.repeat)
                {
                    eventsOnCooldown.Add(new CooldownEvent(validEvent));
                }
                inGameConditionals.Remove(validEvent);
            }

            return validEvent;
        }

        public NightEvent[] GetEvents(GameDataHandler game)
        {
            if (!PlayerPrefs.HasKey("FirstNight"))
            {
                PlayerPrefs.SetInt("FirstNight", 1);
                return new NightEvent[] {intro};
            }

            List<NightEvent> returnEvents = new List<NightEvent>(storyEvents.Count + 1);
            returnEvents.AddRange(storyEvents);
            NightEvent conditionalEvent = GetConditionalEvent(game);
            if (!(conditionalEvent is null))
            {
                returnEvents.Add(conditionalEvent);
            }
            if (returnEvents.Count <= _EVENT_COUNT_FOR_RANDOM_EVENT_)
            {
                returnEvents.Add(GetRandomEvent(game.currentDay));
            }

            storyEvents.Clear();
            CheckEventCooldown();
            return returnEvents.ToArray();
        }

        private void CheckEventCooldown()
        {
            List<CooldownEvent> endedEvents = new List<CooldownEvent>(1);
            foreach (var cooldownEvent in eventsOnCooldown)
            {
                cooldownEvent.Days --;
                if (cooldownEvent.Days == 0)
                {
                    endedEvents.Add(cooldownEvent);
                }
            }
            foreach (var cooldownEvent in endedEvents)
            {
                eventsOnCooldown.Remove(cooldownEvent);
                inGameConditionals.Add(cooldownEvent.Event);
            }
        }

        public void LoadData(GameData data, bool newGame)
        {
            if (newGame || data is null)
            {
                return;
            }
            storyEvents = data.CurrentStoryEvents.
                Select(x => (NightEvent) soDictionary.AllScriptableObjects[x]).
                ToList();
            inGameConditionals = data.CurrentConditionals.
                Select(x => (ConditionalEvent) soDictionary.AllScriptableObjects[x]).
                ToList();
            inGameRandoms = data.CurrentRandomEvents.
                Select(x => (RandomNightEvent) soDictionary.AllScriptableObjects[x]).
                ToList();
            eventsOnCooldown.Clear();
            for (var index = 0; index < data.CooldownEvents.Length; index++)
            {
                var key = data.CooldownEvents[index];
                eventsOnCooldown.Add(new CooldownEvent((ConditionalEvent) soDictionary.AllScriptableObjects[key],
                    data.CooldownDays[index]));
            }
        }

        public void SaveData(ref GameData data)
        {
            if (data == null)
            {
                return;
            }
            data.CurrentStoryEvents = storyEvents.Select(x => x.Id).ToArray();
            data.CurrentConditionals = inGameConditionals.Select(x => x.Id).ToArray();
            data.CurrentRandomEvents = inGameRandoms.Select(x => x.Id).ToArray();
            data.CooldownEvents = eventsOnCooldown.Select(x => x.Event.Id).ToArray();
            data.CooldownDays = eventsOnCooldown.Select(x => x.Days).ToArray();
        }
    }
}