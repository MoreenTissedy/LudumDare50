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
        public List<WrongPotion> AttemptsRecipes;
        public int AttemptsLeft; // VisitorManager

        public List<string> CardPool; // EncounterDeck
        public List<string> CurrentDeck; // EncounterDeck: Don't forget convert LinkedList
        public int LastExtendedPoolNumber;

        public List<string> PriorityCards; //PriorityLaneProvider
        
        // NightEventProvider
        public string[] JoinedNightEvents = Array.Empty<string>();
        public string[] CurrentStoryEvents;
        public string[] CurrentRandomEvents;
        public string[] CurrentConditionals;
        public string[] CooldownEvents;
        public int[] CooldownDays;
        
        public int Fear, Fame, Money; // GameDataHandler
        public int CurrentDay; // GameDataHandler
        public int CardDrawnToday; // GameDataHandler

        public string CurrentEncounter;  // GameDataHandler
        public string CurrentVillager;

        public List<string> StoryTags;  // GameDataHandler
        public int[] FractionData;
        
        public List<string> PotionsTotalOnRun;  //GameDataHandler
        public int WrongPotionsCountOnRun;  //GameDataHandler

        public PotionsBrewedInADay CurrentDayPotions;  //GameDataHandler
        public List<PotionsBrewedInADay> PotionsBrewedInADays;  //GameDataHandler

        public Status Status;  //GameDataHandler
        public GameStateMachine.GamePhase Phase;
        
        public bool DarkStrangerCame, WitchCame, InquisitorCame;

        public GameData(int initialValue)
        {
            AttemptsRecipes = new List<WrongPotion>(10);
            AttemptsLeft = 3;

            CardPool = new List<string>(15);
            CurrentDeck = new List<string>();
            
            Fear = initialValue;
            Fame = initialValue;
            Money = 0;

            CardDrawnToday = 0;
            Phase = GameStateMachine.GamePhase.Visitor;

            CurrentEncounter = null;
            CurrentVillager = null;

            StoryTags = StoryTagHelper.GetMilestones().ToList();
            
            PotionsTotalOnRun = new List<string>();
            WrongPotionsCountOnRun = 0;

            CurrentDayPotions = new PotionsBrewedInADay();
            PotionsBrewedInADays = new List<PotionsBrewedInADay> {CurrentDayPotions};

            Status = new Status();

            DarkStrangerCame = false;
            WitchCame = false;
            InquisitorCame = false;
            
            Debug.Log("GameData has been created");
        }
    }
}
