using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.PlayerLoop;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Encounter Deck", menuName = "Encounter Deck", order = 0)]
    public class EncounterDeck : ScriptableObject
    {
        public Encounter[] startingCards;
        public Encounter[] pool1, pool2, pool3;
        public Queue<Encounter> deck;
        [Header("Deck info")] public Encounter[] deckInfo;
        public List<Encounter> cardPool;

        public void Init()
        {
            deck = new Queue<Encounter>(10);
            foreach (var card in startingCards)
            {
                deck.Enqueue(card);
            }

            cardPool = new List<Encounter>(15);
            cardPool.AddRange(pool1);
        }

        public void NewDayPool(int day)
        {
            if (day == 3)
                cardPool.AddRange(pool2);
            else if (day == 5)
                cardPool.AddRange(pool3);
        }

        /// <summary>
        /// Add X random cards from pool to deck
        /// </summary>
        /// <param name="num">X - number of cards</param>
        public void DealCards(int num)
        {
            for (int i = 0; i < num; i++)
            {
                if (cardPool.Count == 0)
                    return;
                int randomIndex = Random.Range(0, cardPool.Count);
                deck.Enqueue(cardPool[randomIndex]);
                cardPool.RemoveAt(randomIndex);
            }

            deckInfo = deck.ToArray();
        }

        public void AddCardToPool(Encounter card)
        {
            cardPool.Add(card);
        }

        public Encounter GetTopCard()
        {
            var card = deck.Dequeue();
            deckInfo = deck.ToArray();
            return card;
        }
    }
}