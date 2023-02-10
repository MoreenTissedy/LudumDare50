using System;
using System.Collections.Generic;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using Newtonsoft.Json;
using UnityEngine;

namespace Save
{
    [Serializable]
    public class GameData
    {
        public int Fear, Fame, Money; // GameDataHandler

        public int CurrentDay; // GameDataHandler

        public int AttemptsLeft; // VisitorManager

        public Encounter CurrentVisitor;  // GameDataHandler

        public int CardDrawnToday; // GameDataHandler

        public List<Encounter> CardPool; // EncounterDeck
        
        public List<Encounter> CurrentDeck; // EncounterDeck: Don't forget convert LinkedList
        public List<NightEvent> CurrentEvents; // NightEventProvider
        
        public List<string> storyTags;  // GameDataHandler

        public GameData(int initialValue)
        {
            Fear = initialValue;
            Fame = initialValue;
            Money = 0;

            AttemptsLeft = 3;

            CardDrawnToday = 0;
            
            CurrentVisitor = null;

            CardPool = new List<Encounter>(15);
            CurrentDeck = new List<Encounter>();
            CurrentEvents = new List<NightEvent>();
            storyTags = new List<string>(5);
        }
    }
}
