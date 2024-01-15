using System;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using UnityEngine;

namespace Save
{
    [Serializable]
    public class GameData
    {
        public int AttemptsLeft; // VisitorManager

        public List<string> CardPool = new List<string>(); // EncounterDeck
        public List<string> CurrentDeck = new List<string>(); // EncounterDeck: Don't forget convert LinkedList
        public int LastExtendedPoolNumber;

        public List<string> PriorityCards = new List<string>(); //PriorityLaneProvider
        
        // NightEventProvider
        public List<string> JoinedNightEvents = new List<string>();
        public List<string> CurrentStoryEvents = new List<string>();
        public List<string> CurrentRandomEvents = new List<string>();
        public List<string> CurrentConditionals = new List<string>();
        public List<string> CooldownEvents = new List<string>();
        public int[] CooldownDays;
        
        public int Fear, Fame, Money; // GameDataHandler
        public int CurrentDay; // GameDataHandler
        public int CardDrawnToday; // GameDataHandler

        public string CurrentEncounter;  // GameDataHandler
        public string CurrentVillager;

        public List<string> StoryTags ;  // GameDataHandler
        public int[] FractionData;
        
        public List<string> PotionsTotalOnRun = new List<string>();  //GameDataHandler
        public int WrongPotionsCountOnRun;  //GameDataHandler

        public PotionsBrewedInADay CurrentDayPotions;  //GameDataHandler
        public List<PotionsBrewedInADay> PotionsBrewedInADays;  //GameDataHandler

        public Status Status;  //GameDataHandler
        public GameStateMachine.GamePhase Phase;
        
        public bool DarkStrangerCame, WitchCame, InquisitorCame;

        public GameData(int initialValue)
        {
            AttemptsLeft = 3;
            
            Fear = initialValue;
            Fame = initialValue;
            Money = 0;

            CardDrawnToday = 0;
            Phase = GameStateMachine.GamePhase.Visitor;

            CurrentEncounter = null;
            CurrentVillager = null;

            StoryTags = StoryTagHelper.GetMilestones().ToList();

            WrongPotionsCountOnRun = 0;

            CurrentDayPotions = new PotionsBrewedInADay();
            PotionsBrewedInADays = new List<PotionsBrewedInADay> {CurrentDayPotions};

            Status = new Status();
        }

        public void ValidateSave(SODictionary soDictionary)
        {
            ValidateList(CardPool, soDictionary);
            ValidateList(CurrentDeck, soDictionary);
            ValidateList(PriorityCards, soDictionary);
            ValidateList(JoinedNightEvents, soDictionary);
            ValidateList(CurrentStoryEvents, soDictionary);
            ValidateList(CurrentRandomEvents, soDictionary);
            ValidateList(CurrentConditionals, soDictionary);
            ValidateList(CooldownEvents, soDictionary);
        }

        private void ValidateList(IList<string> targetList, SODictionary soDictionary)
        {
            for (int i = 0; i < targetList.Count; i++)
            {
                if (soDictionary.AllScriptableObjects.ContainsKey(targetList[i])) continue;
                
                Debug.LogWarning($"{targetList[i]} not found in dictionary");
                targetList.Remove(targetList[i]);
            }
        }
    }
}
