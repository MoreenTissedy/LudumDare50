using System;
using System.Collections.Generic;
using Save;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Event Provider", menuName = "Event Provider", order = 9)]
    [Serializable]
    public class NightEventProvider : ScriptableObject, IDataPersistence
    {
        private const int _EVENT_COOLDOWN_ = 2;

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
            inGameRandoms.Remove(result);
            return result;
        }

        private NightEvent GetConditionalEvent(GameDataHandler game)
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
            if (returnEvents.Count <= 2)
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
            if(data is null) return;
            //storyEvents = data.CurrentEvents;
            storyEvents = new List<NightEvent>();
            foreach (var eventKey in data.CurrentEvents)
            {
                storyEvents.Add((NightEvent)soDictionary.AllScriptableObjects[eventKey]);
            }
        }

        public void SaveData(ref GameData data)
        {
            if(data == null) return;
            data.CurrentEvents.Clear();
            foreach (var storyEvent in storyEvents)
            {
                data.CurrentEvents.Add(storyEvent.Id);
            }
        }
    }
}