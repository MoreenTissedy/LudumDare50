using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.PlayerLoop;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Encounter Deck", menuName = "Encounter Deck", order = 0)]
    public class EncounterDeck : ScriptableObject
    {
        public Encounter[] startingCards;
        public Encounter[] pool1, pool2, pool3, pool4, pool5;
        public LinkedList<Encounter> deck;
        [Header("Deck info")] public Encounter[] deckInfo;
        
        public List<Encounter> cardPool;

        public void Init()
        {
            deck = new LinkedList<Encounter>();
            foreach (var card in Shuffle(startingCards))
            {
                deck.AddLast(card);
            }

            cardPool = new List<Encounter>(15);
            //cardPool.AddRange(pool1);
        }

        public static Encounter[] Shuffle(Encounter[] deck)
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

        public void NewDayPool(int day)
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
        public void DealCards(int num)
        {
            Debug.Log(cardPool.Count);
            for (int i = 0; i < num; i++)
            {
                Debug.Log(cardPool.Count);
                if (cardPool.Count == 0)
                    return;
                int randomIndex = Random.Range(0, cardPool.Count);
                deck.AddLast(cardPool[randomIndex]);
                cardPool.RemoveAt(randomIndex);
                Debug.Log("one card dealt: "+cardPool.Count);
            }
            Debug.Log(cardPool.Count);

            cardPool.TrimExcess();
            deckInfo = deck.ToArray();
        }

        public void AddCardToPool(Encounter card)
        {
            if (card is null)
                return;
            cardPool.Add(card);
        }

        public void AddToDeck(Encounter card, bool asFirst = false)
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
        
        public Encounter GetTopCard()
        {
            var card = deck.First();
            deck.RemoveFirst();
            deckInfo = deck.ToArray();
            return card;
        }
    }
}