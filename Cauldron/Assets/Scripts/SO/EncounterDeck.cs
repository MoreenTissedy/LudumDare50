using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Encounter Deck", menuName = "Encounter Deck", order = 0)]
    public class EncounterDeck : EncounterDeckBase
    {
        public Encounter[] startingCards;
        public Encounter[] pool1, pool2, pool3, pool4, pool5;
        public LinkedList<Encounter> deck;
        [Header("Deck info")] public Encounter[] deckInfo;

        
        
        public List<Encounter> cardPool;
        private GameState game;

        public override void Init(GameState game)
        {
            this.game = game;
            deck = new LinkedList<Encounter>();
            foreach (var card in Shuffle(startingCards))
            {
                deck.AddLast(card);
            }

            cardPool = new List<Encounter>(15);
            //cardPool.AddRange(pool1);
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
    }
}