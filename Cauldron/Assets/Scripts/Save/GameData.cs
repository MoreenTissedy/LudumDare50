using System;
using System.Collections.Generic;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using UnityEngine;
using UnityEngine.Serialization;

namespace Save
{
    [Serializable]
    public class GameData
    {
        public int AttemptsLeft; // VisitorManager

        public List<string> CardPool; // EncounterDeck
        public List<string> CurrentDeck; // EncounterDeck: Don't forget convert LinkedList

        // NightEventProvider
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
        
        public List<Potions> PotionsTotalOnRun;  //GameDataHandler
        public int WrongPotionsCountOnRun;  //GameDataHandler

        public PotionsBrewedInADay CurrentDayPotions;  //GameDataHandler
        public List<PotionsBrewedInADay> PotionsBrewedInADays;  //GameDataHandler

        public Status Status;  //GameDataHandler
        public GameStateMachine.GamePhase Phase;

        public GameData(int initialValue)
        {
            AttemptsLeft = 3;

            CardPool = new List<string>(15);
            CurrentDeck = new List<string>();
            
            Fear = initialValue;
            Fame = initialValue;
            Money = 0;

            CardDrawnToday = 0;
            Phase = GameStateMachine.GamePhase.VisitorWaiting;

            CurrentEncounter = null;
            CurrentVillager = null;

            StoryTags = new List<string>(5);
            
            PotionsTotalOnRun = new List<Potions>();
            WrongPotionsCountOnRun = 0;

            CurrentDayPotions = new PotionsBrewedInADay();
            PotionsBrewedInADays = new List<PotionsBrewedInADay> {CurrentDayPotions};

            Status = new Status();
            
            Debug.Log("GameData has been created");
        }
    }
}
