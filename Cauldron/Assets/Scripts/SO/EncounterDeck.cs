using System;
using System.Collections.Generic;
using System.Linq;
using Save;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    [CreateAssetMenu]
    public class EncounterDeck : ScriptableObject, IDataPersistence
    {
        public Encounter[] introCards;
        public CardPoolPerDay[] cardPoolsByDay;
        public LinkedList<Encounter> deck;

        [Header("DEBUG")] public Encounter currentCard;
        public Encounter[] deckInfo;
        public List<Encounter> cardPool;

        private GameDataHandler gameDataHandler;
        private SODictionary soDictionary;
        private Encounter loadedCard;

        public List<string> rememberedCards;

        private MainSettings mainSettings;

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
                    if (gameDataHandler.currentDay <= mainSettings.gameplay.daysWithUniqueStartingCards
                        && gameDataHandler.currentRound <= mainSettings.gameplay.roundsWithUniqueStartingCards)
                    {
                        return pool.cards.Except(rememberedCards.Select(x => (Encounter) soDictionary.AllScriptableObjects[x])).ToArray();
                    }

                    return pool.cards;

                }
            }

            return Array.Empty<Encounter>();
        }


        /// <summary>
        /// Form new deck and starting card pool.
        /// </summary>
        public void Init(GameDataHandler game, DataPersistenceManager dataPersistenceManager,
            SODictionary dictionary, MainSettings settings)
        {
            gameDataHandler = game;
            soDictionary = dictionary;
            mainSettings = settings;
            dataPersistenceManager.AddToDataPersistenceObjList(this);

            InitRememberedCards();
        }

        private void InitRememberedCards()
        {
            string rememberedCardsJson = PlayerPrefs.GetString(PrefKeys.UniqueCards);
            if (!string.IsNullOrEmpty(rememberedCardsJson))
            {
                var wrapper = JsonUtility.FromJson<EncounterListWrapper>(rememberedCardsJson);
                rememberedCards.Clear();
                rememberedCards.AddRange(wrapper.encounters);
            }
            else
            {
                rememberedCards = new List<string>();
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
        public void NewDayPool(int day)
        {
            foreach (var card in Shuffle(GetPoolForDay(day)))
            {
                cardPool.Add(card);
            }
        }

        public void DealCardsTo(int target)
        {
            if (target - deck.Count <= 0)
            {
                return;
            }
            DealCards(target - deck.Count);
        }

        /// <summary>
        /// Add X random cards from pool to deck
        /// </summary>
        /// <param name="num">X - number of cards</param>
        public void DealCards(int num)
        {
            //TODO: separate array for story-related cards
            AddStoryCards();

            //ignore story-related cards
            for (int i = 0; i < num; i++)
            {
                if (cardPool.Count == 0)
                    return;
                int randomIndex = 0;
                do
                {
                    randomIndex = Random.Range(0, cardPool.Count);
                } while (!string.IsNullOrEmpty(cardPool[randomIndex].requiredStoryTag));

                if (deck == null) Debug.LogWarning("deck == null");
                deck.AddLast(cardPool[randomIndex]);
                cardPool.RemoveAt(randomIndex);
            }

            cardPool.TrimExcess();
            deckInfo = deck.ToArray();
        }

        public void AddStoryCards()
        {
            //find story-related cards and add them as top-priority above count
            List<Encounter> highPriorityCards = new List<Encounter>(3);

            foreach (Encounter card in cardPool)
            {
                //if(card == null) return;
                if (string.IsNullOrEmpty(card.requiredStoryTag))
                {
                    continue;
                }

                Debug.Log("checking card: " + card.name);
                if (CheckStoryTags(gameDataHandler, card))
                {
                    deck.AddFirst(card);
                    highPriorityCards.Add(card);
                }
            }

            foreach (Encounter highPriorityCard in highPriorityCards)
            {
                Debug.Log("card added as priority " + highPriorityCard.name);
                cardPool.Remove(highPriorityCard);
            }
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

            deckInfo = deck.ToArray();
        }

        public Encounter GetTopCard()
        {
            if (loadedCard != null)
            {
                currentCard = loadedCard;
                loadedCard = null;
            }
            else
            {
                var topCard = deck.First();
                deck.RemoveFirst();
                currentCard = topCard;
            }

            if (gameDataHandler.currentDay < mainSettings.gameplay.daysWithUniqueStartingCards
                && gameDataHandler.currentRound < mainSettings.gameplay.roundsWithUniqueStartingCards
                && !VisitorManager.SPECIALS.Contains(currentCard.villager.name))
            {
                SaveCurrentCardAsUnique();
            }

            return currentCard;
        }

        void SaveCurrentCardAsUnique()
        {
            rememberedCards.Add(currentCard.Id);
            EncounterListWrapper wrapper = new EncounterListWrapper { encounters = rememberedCards };
            string json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(PrefKeys.UniqueCards, json);
            Debug.Log("unique cards: "+json);
        }
        

        public void LoadData(GameData data, bool newGame)
        {
            loadedCard = gameDataHandler.currentCard;

            deck = new LinkedList<Encounter>();
            cardPool = new List<Encounter>(15);

            switch (newGame)
            {
                case true:
                    NewDayPool(0);
                    //if not first time
                    if (PlayerPrefs.GetInt(PrefKeys.CurrentRound) != 0)
                    {
                        DealCards(2);
                        deck.AddFirst(introCards[2]);
                    }
                    else
                    {
                        DealCards(1);
                        deck.AddFirst(introCards[0]);
                        deck.AddLast(introCards[1]);
                    }


                    break;
                case false:
                    cardPool = new List<Encounter>();
                    if (data.CardPool != null)
                    {
                        foreach (var key in data.CardPool)
                        {
                            cardPool.Add((Encounter)soDictionary.AllScriptableObjects[key]);
                        }
                    }

                    if (data.CurrentDeck != null)
                    {
                        List<Encounter> currentDeck = new List<Encounter>();
                        foreach (var key in data.CurrentDeck)
                        {
                            currentDeck.Add((Encounter)soDictionary.AllScriptableObjects[key]);
                        }

                        deck = new LinkedList<Encounter>(currentDeck);
                        Debug.Log("New deck");
                    }

                    break;
            }
        }

        public void SaveData(ref GameData data)
        {
            if (data == null) return;
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

        private static bool CheckStoryTags(GameDataHandler game, Encounter card)
        {
            string[] tags = card.requiredStoryTag.Split(',');
            bool valid = true;
            foreach (var tag in tags)
            {
                if (tag.StartsWith("!"))
                {
                    valid = valid && !game.storyTags.Contains(tag.Trim().TrimStart('!'));
                }
                else
                {
                    valid = valid && game.storyTags.Contains(tag.Trim());
                }
            }

            return valid;
        }
    }
}