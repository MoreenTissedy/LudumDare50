using System;
using UnityEngine;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public IngredientsData ingredientsBook;
        public EncounterDeck cardDeck;
        
        public Status money, fear, fame;
        public int defaultMoneyBonus = 5;
        public float fameMoneyCoef = 0.1f;
        public int defaultFameBonus = 5;

        private int moneyUpdateTotal, fameUpdateTotal, fearUpdateTotal;

        private int currentDay;
        private int cardsDrawnToday;
        public int cardsPerDay = 3;
        public int cardsDealtAtNight = 3;
        
        //stats
        [Header("for info")]
        public Encounter currentCard;
        public List<Potions> rightPotionsTotal;
        public List<Potions> wrongPotionsTotal;

        private void Awake()
        {
            if (instance is null)
                instance = this;
            else
            {
                Debug.LogError("double singleton:"+this.GetType().Name);
            }

            rightPotionsTotal = new List<Potions>(15);
            wrongPotionsTotal = new List<Potions>(15);

            money = new Status();
            fame = new Status();
            fear = new Status();
        }

        private void Start()
        {
            cardDeck.Init();
            DrawCard();
        }

        public void DrawCard()
        {
            currentCard = cardDeck.GetTopCard();
            if (currentCard == null)
            {
                Debug.LogError("Ran out of cards!!!");
                return;
            }
            cardsDrawnToday ++;
            Debug.Log(currentCard.text);
        }

        public void EndEncounter(Potions potion)
        {
            //visual effects
            
            //status update
            money.Value += defaultMoneyBonus;
                           //+ Mathf.FloorToInt(fame.Value*fameMoneyCoef);
            fame.Value += defaultFameBonus;
            
            //compare potion and save bonuses for later
            if (potion == currentCard.requiredPotion)
            {
                moneyUpdateTotal += currentCard.moneyBonus;
                fearUpdateTotal += currentCard.fearBonus;
                fameUpdateTotal += currentCard.fameBonus;
                rightPotionsTotal.Add(potion);
                if (currentCard.bonusCard != null)
                    cardDeck.AddCardToPool(currentCard.bonusCard);
            }
            else
            {
                moneyUpdateTotal += currentCard.moneyPenalty;
                fearUpdateTotal += currentCard.fearPenalty;
                fameUpdateTotal += currentCard.famePenalty;
                wrongPotionsTotal.Add(potion);
                if (currentCard.penaltyCard != null)
                    cardDeck.AddCardToPool(currentCard.penaltyCard);
            }
            
            //draw new card
            if (cardsDrawnToday >= cardsPerDay)
            {
                Night();
            }

            DrawCard();
        }

        private void Night()
        {
            cardDeck.DealCards(cardsDealtAtNight);
            currentDay++;
            cardsDrawnToday = 0;
            
            //status update
            money.Value += moneyUpdateTotal;
            moneyUpdateTotal = 0;
            fear.Value += fearUpdateTotal;
            fearUpdateTotal = 0;
            fame.Value = fameUpdateTotal;
            fameUpdateTotal = 0;
            
            //text message
            Debug.Log("New day!");
        }
    }
}