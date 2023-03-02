using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Save;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Encounter Deck", menuName = "Encounter Deck", order = 0)]
    [Serializable]
    public class EncounterDeck : EncounterDeckBase, IDataPersistence
    {
        public Encounter[] startingCards;
        public Encounter[] pool1, pool2, pool3, pool4, pool5;
        public LinkedList<Encounter> deck;
        [Header("Deck info")] public Encounter[] deckInfo;
        
        public List<Encounter> cardPool;
        private GameDataHandler game;
        private SODictionary soDictionary;

        public override void Init(GameDataHandler game, DataPersistenceManager dataPersistenceManager, SODictionary dictionary)
        {
            this.game = game;
            soDictionary = dictionary;
            dataPersistenceManager.AddToDataPersistenceObjList(this);
        }

        private static Encounter[] Shuffle(Encounter[] deck)
        {
            List<Encounter> deckList = deck.ToList();
            var newDeckList = new List<Encounter>(deckList.Count);
            while (deckList.Count > 0)
            {
                int random = Random.Range(0, deckList.Count);
                newDeckList.Add(deckList[random]);
                deckList.RemoveAt(random);
            }
            return newDeckList.ToArray();
        }

        public override void NewDayPool(int day)
        {
            switch ((day-1)%5)
            {
                case 0:
                    cardPool.AddRange(pool1);
                    break;
                case 1:
                    cardPool.AddRange(pool2);
                    break;
                case 2:
                    cardPool.AddRange(pool3);
                    break;
                case 3:
                    cardPool.AddRange(pool4);
                    break;
                case 4:
                    cardPool.AddRange(pool5);
                    break;
            }
        }

        /// <summary>
        /// Add X random cards from pool to deck
        /// </summary>
        /// <param name="num">X - number of cards</param>
        public override void DealCards(int num)
        {
            for (int i = 0; i < num; i++)
            {
                if (cardPool.Count == 0)
                    return;
                int randomIndex = Random.Range(0, cardPool.Count);
                deck.AddLast(cardPool[randomIndex]);
                cardPool.RemoveAt(randomIndex);
            }

            cardPool.TrimExcess();
            deckInfo = deck.ToArray();
        }

        public override void AddCardToPool(Encounter card)
        {
            if (card is null)
                return;
            cardPool.Add(card);
        }

        public override void AddToDeck(Encounter card, bool asFirst = false)
        {
            if (card is null)
                return;
            if (asFirst)
            {
                deck.AddFirst(card);
            }
            else
            {
                deck.AddLast(card);
            }
        }
        
        public override Encounter GetTopCard()
        {
            Encounter card = null;
            do
            {
                if (card != null)
                {
                    deck.AddLast(card);
                }
                card = deck.First();
                deck.RemoveFirst();
            } 
            while (!string.IsNullOrEmpty(card.requiredStoryTag) &&
                   !game.storyTags.Contains(card.requiredStoryTag));

            deckInfo = deck.ToArray();
            return card;
        }

        public override void LoadData(GameData data, bool newGame)
        {
            cardPool = new List<Encounter>();
            if (data.CardPool != null)
            {
                foreach (var key in data.CardPool)
                {
                    cardPool.Add((Encounter)soDictionary.AllScriptableObjects[key]);
                }
                Debug.Log("New CardPool");
            }


            if (data.CurrentDeck != null)
            {
                List<Encounter> currentDeck = new List<Encounter>();
                foreach (var key in data.CurrentDeck)
                {
                    currentDeck.Add((Encounter)soDictionary.AllScriptableObjects[key]);
                }

                deck = new LinkedList<Encounter>(currentDeck);
                
                Debug.Log("New Deck");
            }

            if (deck.Count == 0)
            {
                foreach (var card in Shuffle(startingCards))
                {
                    deck.AddLast(card);
                }
            }
        }

        public override void SaveData(ref GameData data)
        {
            data.CardPool.Clear();
            foreach (var card in cardPool)
            {
                data.CardPool.Add(card.Id);
            }
            data.CurrentDeck.Clear();
            foreach (var card in deck)
            {
                data.CurrentDeck.Add(card.Id);
            }
        }
    }
}