using System;
using System.Collections.Generic;
using CauldronCodebase.GameStates;
using UnityEngine;

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
        public int currentRound;
        public GameStateMachine.GamePhase gamePhase;
        public Encounter currentCard;
        public List<Potions> potionsTotal;
        public int wrongPotionsCount;
        public int wrongExperiments = 0;  //no need to save

        //TODO: separate entities
        public List<string> storyTags;
        
        public FractionStatus fractionStatus;
        public bool fractionEventTriggered;
        
        public EncounterDeck currentDeck;

        public MainSettings.StatusBars statusSettings;
        private MainSettings.Gameplay gameplaySettings;
        PlayerProgressProvider progressProvider;
        public event Action<Statustype, int> StatusChanged;

        private PotionsBrewedInADay currentDayPotions;
        private List<PotionsBrewedInADay> potionsBrewedInADays;

        public List<Potions> PotionsOnLastDays = new List<Potions>(15);
        public int WrongPotionsOnLastDays;
        public int DayCountThreshold = 2;

        private SODictionary soDictionary;
        
        public void Init(MainSettings settings, EncounterDeck deck, DataPersistenceManager dataManager,
                         SODictionary dictionary, PlayerProgressProvider progressProvider)
        {
            soDictionary = dictionary;
            
            dataManager.AddToDataPersistenceObjList(this);

            statusSettings = settings.statusBars;
            
            currentDeck = deck;

            fractionStatus = new FractionStatus();
            this.progressProvider = progressProvider;
            
            currentRound = progressProvider.CurrentRound;
        }

        public bool IsEnoughMoneyForRumours()
        {
            return Money >= statusSettings.CovenCost;
        }

        public void BuyRumour()
        {
            money -= statusSettings.CovenCost;
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
            if (statValue > ((type == Statustype.Money) ? 5000 : statusSettings.Total))
                statValue = (type == Statustype.Money) ? 5000 : statusSettings.Total;
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
            StatusChanged?.Invoke(type, num);
            return statValue;
        }

        public void SetCurrentCard(Encounter card)
        {
            currentCard = card;
        }

        public int Add(Statustype type, int value)
        {
            return Set(type, value + Get(type));
        }

        public bool ChangeThreshold(Statustype type, bool high)
        {
            if (ReachedMaxThreshold(type, high))
            {
                return false;
            }
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
                    return true;
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
                    return true;
            }
            return false;
        }
        
        public bool ReachedMaxThreshold(Statustype type, bool high)
        {
            switch (type)
            {
                case Statustype.Fear:
                    if (high)
                    {
                        return status.FearThresholdHigh == statusSettings.GetMaxThreshold;
                    }
                    else
                    {
                        return status.FearThresholdLow == statusSettings.GetMinThreshold;
                    }
                case Statustype.Fame:
                    if (high)
                    {
                        return status.FameThresholdHigh == statusSettings.GetMaxThreshold;
                    }
                    else
                    {
                        return status.FameThresholdLow == statusSettings.GetMinThreshold;
                    }
            }
            return false;
        }

        public void AddPotion(Potions potion, bool wrong)
        {
            potionsTotal.Add(potion); // for global statistic
            currentDayPotions.PotionsList.Add(potion.ToString());
            if (wrong)
            {
                Debug.Log("wrong potion");
                wrongPotionsCount++; // for global statistic
                currentDayPotions.WrongPotions++;
            }
        }

        public void NextDay()
        {
            currentDay++;
            cardsDrawnToday = 0;
            potionsBrewedInADays.Add(currentDayPotions);
            if (potionsBrewedInADays.Count > DayCountThreshold)
            {
                potionsBrewedInADays.RemoveAt(0);
            }

            currentDayPotions = new PotionsBrewedInADay();
        }

        public void RememberRound()
        {
            currentRound += 1;
            progressProvider.SaveCurrentRound(currentRound);
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
                    PotionsOnLastDays.Add((Potions)Enum.Parse(typeof(Potions), potion));
                }
            }
        }

        public void LoadData(GameData data, bool newGame)
        {
            //refaaactor me
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
            fractionStatus.Load(data.FractionData);
            fractionEventTriggered = data.FractionEventTriggered;

            if (string.IsNullOrEmpty(data.CurrentEncounter))
            {
                currentCard = null;
            }
            else
            {
                currentCard = (Encounter)soDictionary.AllScriptableObjects[data.CurrentEncounter];
            }

            potionsTotal = new List<Potions>();
            foreach (var potion in data.PotionsTotalOnRun)
            {
                potionsTotal.Add((Potions)Enum.Parse(typeof(Potions), potion));
            }
            
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
            data.FractionData = fractionStatus.Save();
            data.FractionEventTriggered = fractionEventTriggered;

            if (currentCard == null)
            {
                data.CurrentEncounter = string.Empty;
            }
            else
            {
                data.CurrentEncounter = currentCard.name;
            }

            data.PotionsTotalOnRun.Clear();
            foreach (var potion in potionsTotal)
            {
                data.PotionsTotalOnRun.Add(potion.ToString());
            }
            
            data.WrongPotionsCountOnRun = wrongPotionsCount;

            data.CurrentDayPotions = currentDayPotions;
            data.PotionsBrewedInADays = potionsBrewedInADays;
        }
    }
}