using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    [CreateAssetMenu]
    public class EncounterDeckAutoFilled : EncounterDeckBase
    {
        public Encounter[] introCards;
        public CardPoolPerDay[] cardPoolsByDay;
        public LinkedList<Encounter> deck;
        [Header("DEBUG")] 
        public Encounter[] deckInfo;
        public List<Encounter> cardPool;

        private GameState game;

        [Serializable]
        public struct CardPoolPerDay
        {
            [HideInInspector] public string title;
            public int day;
            public Encounter[] cards;

            public CardPoolPerDay(int day, Encounter[] cards)
            {
                this.day = day;
                this.cards = cards;
                title = $"Day {day}: {cards.Length} cards";
            }
        }

        private Encounter[] GetPoolForDay(int day)
        {
            foreach (var pool in cardPoolsByDay)
            {
                if (pool.day == day)
                {
                    return pool.cards;
                }
            }
            return new Encounter[0];
        }
        
        /// <summary>
        /// Form new deck and starting card pool.
        /// </summary>
        public override void Init(GameState game)
        {
            this.game = game;
            deck = new LinkedList<Encounter>();
            cardPool = new List<Encounter>(15);
            NewDayPool(0);
            DealCards(1);
            
            //if first time
            if (PlayerPrefs.HasKey("FirstTime"))
            {
                deck.AddFirst(introCards[2]);
                DealCards(1);
            }
            else
            {
                deck.AddFirst(introCards[0]);
                deck.AddLast(introCards[1]);
                PlayerPrefs.SetInt("FirstTime", 1);
            }
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

        /// <summary>
        /// Form card pool, adding cards for the given 'day' (card set number).
        /// </summary>
        /// <param name="day">Day — card set number</param>
        public override void NewDayPool(int day)
        {
            foreach (var card in Shuffle(GetPoolForDay(day)))
            {
                cardPool.Add(card);
            }
        }

        /// <summary>
        /// Add X random cards from pool to deck
        /// </summary>
        /// <param name="num">X - number of cards</param>
        public override void DealCards(int num)
        {
            //find story-related cards and add them as top-priority above count
            List<Encounter> highPriorityCards = new List<Encounter>(3);
            if (game.storyTags != null) Debug.Log("story tags: "+string.Join(" ", game.storyTags));
            foreach (Encounter card in cardPool)
            {
                if (string.IsNullOrEmpty(card.requiredStoryTag))
                {
                    continue;
                }
                Debug.Log("checking card: "+card.requiredStoryTag);
                if (CheckStoryTags(game, card))
                {
                    deck.AddFirst(card);
                    highPriorityCards.Add(card);
                }
            }
            foreach (Encounter highPriorityCard in highPriorityCards)
            {
                Debug.Log("card added as priority "+highPriorityCard.name);
                cardPool.Remove(highPriorityCard);
            }
            
            //ignore story-related cards
            for (int i = 0; i < num; i++)
            {
                if (cardPool.Count == 0)
                    return;
                int randomIndex = 0;
                do
                {
                    randomIndex = Random.Range(0, cardPool.Count);
                } 
                while (!string.IsNullOrEmpty(cardPool[randomIndex].requiredStoryTag));
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
            deckInfo = deck.ToArray();
        }
        
        public override Encounter GetTopCard()
        {
            Encounter card = null;
            bool valid = false;
            int numCards = deck.Count;
            do
            {
                if (card != null)
                {
                    Debug.LogWarning($"Drawn card {card.name} with tag {card.requiredStoryTag}, returned it to deck");
                    deck.AddLast(card);
                }
                card = deck.First();
                deck.RemoveFirst();
                numCards--;
                if (string.IsNullOrEmpty(card.requiredStoryTag))
                {
                    break;
                }
                valid = CheckStoryTags(game, card);
                if (numCards == 0)
                {
                    DealCards(1);
                }
            } 
            while (!valid && numCards > 0);

            deckInfo = deck.ToArray();
            return card;
        }

        private static bool CheckStoryTags(GameState game, Encounter card)
        {
            string[] tags = card.requiredStoryTag.Split(',');
            bool valid = true;
            foreach (var tag in tags)
            {
                valid = valid && game.storyTags.Contains(tag.Trim());
            }
            return valid;
        }
    }
}