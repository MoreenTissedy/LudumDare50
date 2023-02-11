using System;
using System.Collections.Generic;
using Save;
using UnityEngine;
using Zenject;

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
        [Header("Readonly")]
        public List<NightEvent> storyEvents;
        public List<ConditionalEvent> inGameConditionals;
        public List<CooldownEvent> eventsOnCooldown;

        public void Init(DataPersistenceManager dataPersistenceManager)
        {
            eventsOnCooldown = new List<CooldownEvent>(5);
            inGameConditionals = new List<ConditionalEvent>(conditionalEvents.Count);
            inGameConditionals.AddRange(conditionalEvents);
            dataPersistenceManager.AddToDataPersistenceObjList(this);
            //storyEvents.Clear();
        }

        private NightEvent CheckConditions(GameDataHandler game)
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
            List<NightEvent> returnEvents = new List<NightEvent>(storyEvents.Count+1);
            returnEvents.AddRange(storyEvents);
            NightEvent conditionalEvent = CheckConditions(game);
            if (!(conditionalEvent is null))
            {
                returnEvents.Add(conditionalEvent);
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
            storyEvents = data.CurrentEvents;
        }

        public void SaveData(ref GameData data)
        {
            if(data == null) return;
            
            data.CurrentEvents = storyEvents;
        }
    }
}