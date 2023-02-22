using System;
using System.Collections.Generic;
using CauldronCodebase;

namespace Save
{
    [Serializable]
    public class GameData
    {
        public int AttemptsLeft; // VisitorManager

        public List<Encounter> CardPool; // EncounterDeck
        public List<Encounter> CurrentDeck; // EncounterDeck: Don't forget convert LinkedList

        public List<NightEvent> CurrentEvents; // NightEventProvider
        
        public int Fear, Fame, Money; // GameDataHandler
        public int CurrentDay; // GameDataHandler
        public int CardDrawnToday; // GameDataHandler

        public Encounter CurrentVisitor;  // GameDataHandler
        public Villager CurrentVillager;

        public List<string> StoryTags;  // GameDataHandler
        
        public List<Potions> PotionsTotalOnRun;  //GameDataHandler
        public int WrongPotionsCountOnRun;  //GameDataHandler

        public PotionsBrewedInADay CurrentDayPotions;  //GameDataHandler
        public List<PotionsBrewedInADay> PotionsBrewedInADays;  //GameDataHandler

        public Status Status;  //GameDataHandler


        public GameData(int initialValue)
        {
            AttemptsLeft = 3;

            CardPool = new List<Encounter>(15);
            CurrentDeck = new List<Encounter>();

            CurrentEvents = new List<NightEvent>();
            
            Fear = initialValue;
            Fame = initialValue;
            Money = 0;

            CardDrawnToday = 0;

            CurrentVisitor = null;
            CurrentVillager = null;

            StoryTags = new List<string>(5);
            
            PotionsTotalOnRun = new List<Potions>();
            WrongPotionsCountOnRun = 0;

            CurrentDayPotions = new PotionsBrewedInADay();
            PotionsBrewedInADays = new List<PotionsBrewedInADay> {CurrentDayPotions};

            Status = new Status();
        }
    }
}
