using System;
using System.Collections.Generic;
using Save;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace CauldronCodebase
{
    //Class holds the game state and can be used for saving and loading
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

        //status struct?
        private int fearThresholdLow;
        private int fearThresholdHigh;
        private int fameThresholdLow;
        private int fameThresholdHigh;
        
        public int currentDay = 1;
        public int cardsDrawnToday;
        public Encounter currentCard;
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

        public bool loadIgnoreSaveFile;
        
        [Inject]
        public void Construct(MainSettings settings, EncounterDeckBase deck, NightEventProvider events, DataPersistenceManager dataPersistenceManager)
        {
            this.dataPersistenceManager = dataPersistenceManager;
            dataPersistenceManager.AddToDataPersistenceObjList(this);

            statusSettings = settings.statusBars;
            fearThresholdLow = (int)(statusSettings.InitialThreshold / 100 * statusSettings.Total);
            fameThresholdLow = fearThresholdLow;
            fameThresholdHigh = (int)((100f - statusSettings.InitialThreshold) / 100 * statusSettings.Total);
            fearThresholdHigh = fameThresholdHigh;

            currentDeck = deck;
            currentDeck.Init(this, this.dataPersistenceManager);
            currentEvents = events;
            currentEvents.Init(this.dataPersistenceManager);
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
                    return high ? fearThresholdHigh : fearThresholdLow;
                case Statustype.Fame:
                    return high ? fameThresholdHigh : fameThresholdLow;
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
                        fearThresholdHigh += statusSettings.ThresholdDecrement;
                        fearThresholdHigh = Mathf.Clamp(fearThresholdHigh, 0, statusSettings.GetMaxThreshold);
                        Debug.Log("next high fear at " + fearThresholdHigh);
                    }
                    else
                    {
                        fearThresholdLow -= statusSettings.ThresholdDecrement;
                        fearThresholdLow = Mathf.Clamp(fearThresholdLow, statusSettings.GetMinThreshold, statusSettings.Total);
                        Debug.Log("next low fear at " + fearThresholdLow);
                    }
                    break;
                case Statustype.Fame:
                    if (high)
                    {
                        fameThresholdHigh += statusSettings.ThresholdDecrement;
                        fameThresholdHigh = Mathf.Clamp(fameThresholdHigh, 0, statusSettings.GetMaxThreshold);
                        Debug.Log("next high fame at " + fameThresholdHigh);
                    }
                    else
                    {
                        fameThresholdLow -= statusSettings.ThresholdDecrement;
                        fameThresholdLow = Mathf.Clamp(fameThresholdLow, statusSettings.GetMinThreshold, statusSettings.Total);
                        Debug.Log("next low fame at " + fameThresholdLow);
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
            loadIgnoreSaveFile = newGame;

            fame = data.Fame;
            fear = data.Fear;
            money = data.Money;
            currentDay = data.CurrentDay;
            cardsDrawnToday = data.CardDrawnToday;
            storyTags = data.StoryTags;
            currentCard = data.CurrentVisitor;

            potionsTotal = data.PotionsTotalOnRun;
            wrongPotionsCount = data.WrongPotionsCountOnRun;

            currentDayPotions = data.CurrentDayPotions;
            potionsBrewedInADays = data.PotionsBrewedInADays;
        }

        public void SaveData(ref GameData data)
        {
            if(data == null) return;
            
            data.Fame = fame;
            data.Fear = Fear;
            data.Money = money;
            data.CurrentDay = currentDay;
            data.CardDrawnToday = cardsDrawnToday;
            data.StoryTags = storyTags;
            data.CurrentVisitor = currentCard;

            data.PotionsTotalOnRun = potionsTotal;
            data.WrongPotionsCountOnRun = wrongPotionsCount;

            data.CurrentDayPotions = currentDayPotions;
            data.PotionsBrewedInADays = potionsBrewedInADays;
        }
    }
    

    [Serializable]
    public class PotionsBrewedInADay
    {
        public List<Potions> PotionsList = new List<Potions>(15);
        public int WrongPotions = 0;
    }
}