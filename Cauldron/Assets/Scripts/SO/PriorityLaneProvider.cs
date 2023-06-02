using System.Collections.Generic;
using System.Linq;
using Save;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    [CreateAssetMenu]
    public class PriorityLaneProvider : ScriptableObject, IDataPersistence
    {
        public const string HIGH_FAME = "high fame";
        public const string LOW_FAME = "low fame";
        public const string HIGH_FEAR = "high fear";
        public const string LOW_FEAR = "low fear";
        
        public Encounter[] priorityCards;
        
        private EncounterDeck deck;
        private SODictionary dictionary;

        public List<Encounter> highFame;
        public List<Encounter> highFear;
        public List<Encounter> lowFame;
        public List<Encounter> lowFear;

        public void Init(EncounterDeck deck, SODictionary dictionary, DataPersistenceManager dataPersistenceManager)
        {
            this.deck = deck;
            this.dictionary = dictionary;
            dataPersistenceManager.AddToDataPersistenceObjList(this);
        }

        private void InitInternal(Encounter[] cards)
        {
            highFame = new List<Encounter>(3);
            highFear = new List<Encounter>(3);
            lowFame = new List<Encounter>(1);
            lowFear = new List<Encounter>(1);
            foreach (Encounter card in cards)
            {
                var tags = card.requiredStoryTag.Split(',');
                if (tags.Contains(HIGH_FAME))
                {
                    highFame.Add(card);
                }
                else if (tags.Contains(HIGH_FEAR))
                {
                    highFear.Add(card);
                }
                else if (tags.Contains(LOW_FAME))
                {
                    lowFame.Add(card);
                }
                else if (tags.Contains(LOW_FEAR))
                {
                    lowFear.Add(card);
                }
            }
        }

        public Encounter GetRandomCard(string tag)
        {
            var set = GetCardSet(tag);
            Encounter card;
            int random;
            for (int i = 0; i < 10; i++) 
            {
                random = Random.Range(0, set.Count);
                card = set[random];
                if (deck.CheckStoryTags(card))
                {
                    set.RemoveAt(random);
                    return card;
                }
            }
            return null;
        }
        
        public List<Encounter> GetCardSet(string tag)
        {
            if (tag == HIGH_FAME)
            {
                return highFame;
            }
            else if (tag == HIGH_FEAR)
            {
                return highFear;
            }
            else if (tag == LOW_FAME)
            {
                return lowFame;
            }
            else if (tag == LOW_FEAR)
            {
                return lowFear;
            }
            return new List<Encounter>();
        }

        public void LoadData(GameData data, bool newGame)
        {
            if (!newGame)
            {
                InitInternal(priorityCards);
                return;
            }
            InitInternal(data.PriorityCards.Select((x => (Encounter)dictionary.AllScriptableObjects[x])).ToArray());
        }

        public void SaveData(ref GameData data)
        {
            data.PriorityCards = highFame.Union(highFear).Union(lowFame).Union(lowFear).Select(x => x.name).ToList();
        }
    }
}