using System;
using System.Collections.Generic;
using CauldronCodebase.GameStates;
using Save;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    [Serializable]
    public class GameDataHandler : MonoBehaviour,IDataPersistence
    {
        private int fame;
        private int fear;
        private int money;
        public int Fame
        {
            get => fame;
            set => Set(Statustype.Fame, value);
        }
        public int Fear
        {
            get => fear;
            set => Set(Statustype.Fear, value);
        }
        public int Money
        {
            get => money;
            set => Set(Statustype.Money, value);
        }

        private Status status;
        
        public int currentDay = 0;
        public int cardsDrawnToday;
        public GameStateMachine.GamePhase gamePhase;
        public Encounter currentCard;
        public Villager currentVillager;
        public List<Potions> potionsTotal;
        public int wrongPotionsCount;

        //TODO: separate entities
        public List<string> storyTags;
        public EncounterDeckBase currentDeck;
        public NightEventProvider currentEvents;

        private MainSettings.StatusBars statusSettings;

        private DataPersistenceManager dataPersistenceManager;
        public event Action StatusChanged;
        
        private PotionsBrewedInADay currentDayPotions;
        private List<PotionsBrewedInADay> potionsBrewedInADays;

        public List<Potions> PotionsOnLastDays = new List<Potions>(15);
        public int WrongPotionsOnLastDays;
        public int DayCountThreshold = 2;

        private SODictionary soDictionary;
        
        [Inject]
        public void Construct(MainSettings settings, EncounterDeckBase deck, NightEventProvider events, DataPersistenceManager dataManager, SODictionary dictionary)
        {
            soDictionary = dictionary;

            dataPersistenceManager = dataManager;
            dataManager.AddToDataPersistenceObjList(this);

            statusSettings = settings.statusBars;

            currentDeck = deck;
            currentDeck.Init(this, dataPersistenceManager, dictionary);
            currentEvents = events;
            currentEvents.Init(dataPersistenceManager, dictionary);
        }

        public void AddTag(string tag)
        {
            if (!storyTags.Contains(tag))
            {
                storyTags.Add(tag);
            }
        }
        
        public int Get(Statustype type)
        {
            switch (type)
            {
                case Statustype.Money:
                    return money;
                case Statustype.Fear:
                    return fear;
                case Statustype.Fame:
                    return fame;
            }
            return -1000;
        }

        public int GetThreshold(Statustype type, bool high)
        {
            switch (type)
            {
                case Statustype.Fear:
                    return high ? status.FearThresholdHigh : status.FearThresholdLow;
                case Statustype.Fame:
                    return high ? status.FameThresholdHigh : status.FameThresholdLow;
            }

            return -1000;
        }
        
        private int Set(Statustype type, int newValue)
        {
            int statValue = Get(type);
            int num = newValue - statValue;
            if (num == 0)
                return statValue;
            if (type == Statustype.Money && num < 0)
                return statValue;
            statValue += num;
            if (statValue > statusSettings.Total)
                statValue = statusSettings.Total;
            else if (statValue < 0)
                statValue = 0;
            switch (type)
            {
                case Statustype.Money:
                    money = statValue;
                    break;
                case Statustype.Fear:
                    fear = statValue;
                    break;
                case Statustype.Fame:
                    fame = statValue;
                    break;
            }
            StatusChanged?.Invoke();
            return statValue;
        }

        public int Add(Statustype type, int value)
        {
            return Set(type, value + Get(type));
        }

        public void ChangeThreshold(Statustype type, bool high)
        {
            switch (type)
            {
                case Statustype.Fear:
                    if (high)
                    {
                        status.FearThresholdHigh += statusSettings.ThresholdDecrement;
                        status.FearThresholdHigh = Mathf.Clamp(status.FearThresholdHigh, 0, statusSettings.GetMaxThreshold);
                        Debug.Log("next high fear at " + status.FearThresholdHigh);
                    }
                    else
                    {
                        status.FearThresholdLow -= statusSettings.ThresholdDecrement;
                        status.FearThresholdLow = Mathf.Clamp(status.FearThresholdLow, statusSettings.GetMinThreshold, statusSettings.Total);
                        Debug.Log("next low fear at " + status.FearThresholdLow);
                    }
                    break;
                case Statustype.Fame:
                    if (high)
                    {
                        status.FameThresholdHigh += statusSettings.ThresholdDecrement;
                        status.FameThresholdHigh = Mathf.Clamp(status.FameThresholdHigh, 0, statusSettings.GetMaxThreshold);
                        Debug.Log("next high fame at " + status.FameThresholdHigh);
                    }
                    else
                    {
                        status.FameThresholdLow -= statusSettings.ThresholdDecrement;
                        status.FameThresholdLow = Mathf.Clamp(status.FameThresholdLow, statusSettings.GetMinThreshold, statusSettings.Total);
                        Debug.Log("next low fame at " + status.FameThresholdLow);
                    }
                    break;
            }
        }

        public void AddPotion(Potions potion, bool wrong)
        {
            potionsTotal.Add(potion); // for global statistic
            currentDayPotions.PotionsList.Add(potion);
            if (wrong)
            {
                wrongPotionsCount++; // for global statistic
                currentDayPotions.WrongPotions++;
            }
        }

        public void NextDay()
        {
            currentDay++;
            cardsDrawnToday = 0;
            currentDayPotions = new PotionsBrewedInADay();
            potionsBrewedInADays.Add(currentDayPotions);
            if (potionsBrewedInADays.Count > DayCountThreshold)
            {
                potionsBrewedInADays.RemoveAt(0);
            }
        }
        
        public void CalculatePotionsOnLastDays()
        {
            WrongPotionsOnLastDays = 0;
            PotionsOnLastDays.Clear();
            
            foreach (var day in potionsBrewedInADays)
            {
                WrongPotionsOnLastDays += day.WrongPotions;
                foreach (var potion in day.PotionsList)
                {
                    PotionsOnLastDays.Add(potion);
                }
            }
        }

        public void LoadData(GameData data, bool newGame)
        {
            if(data is null) return;

            status = data.Status;
            if (newGame)
            {
                int highThreshold = (int)((100f - statusSettings.InitialThreshold) / 100 * statusSettings.Total);
                int lowThreshold = (int)(statusSettings.InitialThreshold / 100 * statusSettings.Total);

                status.FameThresholdHigh = highThreshold;
                status.FearThresholdHigh = highThreshold;
                status.FameThresholdLow = lowThreshold;
                status.FearThresholdLow = lowThreshold;
            }

            fame = data.Fame;
            fear = data.Fear;
            money = data.Money;
            currentDay = data.CurrentDay;
            cardsDrawnToday = data.CardDrawnToday;
            gamePhase = data.Phase;
            storyTags = data.StoryTags;

            if (string.IsNullOrEmpty(data.CurrentEncounter))
            {
                currentCard = null;
            }
            else
            {
                currentCard = (Encounter)soDictionary.AllScriptableObjects[data.CurrentEncounter];
            }
            
            if (string.IsNullOrEmpty(data.CurrentVillager))
            {
                currentVillager = null;
            }
            else
            {
                currentVillager = (Villager)soDictionary.AllScriptableObjects[data.CurrentVillager];
            }
            
            potionsTotal = data.PotionsTotalOnRun;
            wrongPotionsCount = data.WrongPotionsCountOnRun;

            currentDayPotions = data.CurrentDayPotions;
            potionsBrewedInADays = data.PotionsBrewedInADays;
        }

        public void SaveData(ref GameData data)
        {
            if(data == null) return;
            data.Status = status;
            
            data.Fame = fame;
            data.Fear = Fear;
            data.Money = money;
            data.CurrentDay = currentDay;
            data.CardDrawnToday = cardsDrawnToday;
            data.StoryTags = storyTags;
            data.Phase = gamePhase;

            if (currentVillager != null)
            {
                data.CurrentVillager = currentVillager.Id;
            }

            if (currentCard != null)
            {
                data.CurrentEncounter = currentCard.Id;
            }
            
            data.PotionsTotalOnRun = potionsTotal;
            data.WrongPotionsCountOnRun = wrongPotionsCount;

            data.CurrentDayPotions = currentDayPotions;
            data.PotionsBrewedInADays = potionsBrewedInADays;
        }
    }
}