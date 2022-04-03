using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public IngredientsData ingredientsBook;
        public EncounterDeck cardDeck;
        public PotionPopup potionPopup;
        public NightPanel nightPanel;
        public EndingScreen endingPanel;
        public Canvas textCanvas;
        public Text textField;
        
        public Status money, fear, fame;
        public float fameMoneyCoef = 0.1f;
        public int statusBarsMax = 100;
        public int statusBarsStart = 20;

        private int moneyUpdateTotal, fameUpdateTotal, fearUpdateTotal;

        private int currentDay = 1;
        private int cardsDrawnToday;
        public int cardsPerDay = 3;
        public int cardsDealtAtNight = 3;
        public float nightDelay = 4f;
        public float villagerDelay = 2f;

        [Tooltip("High money, high fame, high fear, low fame, low fear")]
        public Ending[] endings;
        public int threshold = 70;
        public Encounter[] highFameCards, highFearCards;

        [Tooltip("A list of conditions for total potions brewed checks that happen at night.")]
        public List<NightCondition> nightConditions;
        
        //stats
        [Header("for info")]
        public Encounter currentCard;
        public List<Potions> potionsTotal;

        public bool gameEnded;

        private void Awake()
        {
            // if (instance is null)
            //     instance = this;
            // else
            // {
            //     Debug.LogError("double singleton:"+this.GetType().Name);
            // }
            instance = this;

            potionsTotal = new List<Potions>(15);

            money = new Status();
            fame = new Status();
            fear = new Status();
            
            HideText();
        }

        public void ShowText(string text)
        {
            textCanvas.gameObject.SetActive(true);
            textField.text = text;
        }

        public void HideText()
        {
            textCanvas.gameObject.SetActive(false);
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
            currentCard.Init();
            cardsDrawnToday ++;
            Debug.Log(currentCard.text);
            ShowText(currentCard.text);
            ChangeVisitor.instance.Enter(currentCard.actualVillager);
        }

        public void EndEncounter(Potions potion)
        {
            ChangeVisitor.instance.Exit();
            HideText();
            
            potionPopup.Show(RecipeBook.instance.GetRecipeForPotion(potion));
            potionsTotal.Add(potion);

            //Debug.Log(money.Value + defaultMoneyBonus);
            //status update
            if (currentCard.actualVillager != null) 
            {
                money.Add(currentCard.actualVillager.moneyBonus + Mathf.FloorToInt(fame.Value() * fameMoneyCoef));
                fame.Add(currentCard.actualVillager.fameBonus);
                fear.Add(currentCard.actualVillager.fearBonus);
            }
            //Debug.Log(money.Value);
            
            //compare potion and save bonuses for later
            if (potion == currentCard.requiredPotion)
            {
                moneyUpdateTotal += currentCard.moneyBonus;
                fearUpdateTotal += currentCard.fearBonus;
                fameUpdateTotal += currentCard.fameBonus;
                if (currentCard.bonusCard.Length>0)
                    cardDeck.AddCardToPool(Encounter.GetRandom(currentCard.bonusCard));
            }
            else if (currentCard.useSecondVariant && potion == currentCard.requiredPotion2)
            {
                moneyUpdateTotal += currentCard.moneyBonus2;
                fearUpdateTotal += currentCard.fearBonus2;
                fameUpdateTotal += currentCard.fameBonus2;
                if (currentCard.bonusCard.Length>0)
                    cardDeck.AddCardToPool(Encounter.GetRandom(currentCard.bonusCard2));
            }
            else
            {
                moneyUpdateTotal += currentCard.moneyPenalty;
                fearUpdateTotal += currentCard.fearPenalty;
                fameUpdateTotal += currentCard.famePenalty;
                if (currentCard.penaltyCard.Length>0)
                    cardDeck.AddCardToPool(Encounter.GetRandom(currentCard.penaltyCard));
            }
            
            //villager exits
            
            //draw new card
            if (cardsDrawnToday >= cardsPerDay)
            {
                StartCoroutine(StartNewDay());
            }
            else
            {
                StartCoroutine(DrawCardWithDelay());
            }

        }

        private IEnumerator DrawCardWithDelay()
        {
            yield return new WaitForSeconds(villagerDelay);
            potionPopup.Hide();
            DrawCard();
        }

        string NightChecks()
        {
            string text = String.Empty;
            List<NightCondition> toRemove = new List<NightCondition>(3);
            foreach (var condition in nightConditions)
            {
                if (potionsTotal.Count(x => x == condition.type) < condition.threshold)
                    continue;
                text += condition.flavourText + " ";
                moneyUpdateTotal += condition.moneyModifier;
                fearUpdateTotal += condition.fearModifier;
                fameUpdateTotal += condition.fameModifier;
                if (condition.bonusCard != null)
                    cardDeck.AddCardToPool(condition.bonusCard);
                toRemove.Add(condition);
            }

            foreach (var condition in toRemove)
            {
                nightConditions.Remove(condition);
            }

            if (text == String.Empty)
            {
                int rnd = Random.Range(0, 2);
                switch (rnd)
                {
                    case 0:
                    text = "Ничего необычного.";
                    break;
                    case 1:
                    text = "Ночь спокойна и тиха.";
                    break;
                }
            }

            return text;
        }

        IEnumerator EndGame()
        {
            gameEnded = true;
            yield return new WaitUntil(() => Input.anyKeyDown);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
        
        void NightStatusChecks()
        {
            //endings
            //[Tooltip("High money, high fame, high fear, low fame, low fear")]
            bool endingReached = false;
            if (fame.Value() >= statusBarsMax)
            {
                endingPanel.Show(endings[1]);
                endingReached = true;
            }
            else if (fear.Value() >= statusBarsMax)
            {
                endingPanel.Show(endings[2]);
                endingReached = true;
            }
            else if (money.Value() >= statusBarsMax)
            {
                endingPanel.Show(endings[0]);
                endingReached = true;

            }
            else if (fame.Value() <= 0)
            {
                endingPanel.Show(endings[3]);
                endingReached = true;
            }
            else if (fear.Value() <= 0)
            {
                endingPanel.Show(endings[4]);
                endingReached = true;
            }

            if (endingReached)
            {
                StartCoroutine(EndGame());
                return;
            }
            
            //high status cards
            if (fame.Value() > threshold)
            {
                cardDeck.AddToDeck(Encounter.GetRandom(highFameCards));
            }

            if (fear.Value() > threshold)
            {
                cardDeck.AddToDeck(Encounter.GetRandom(highFearCards));
            }
        }
        
        private IEnumerator StartNewDay()
        {
            yield return new WaitForSeconds(villagerDelay);
            
            potionPopup.Hide();
            Witch.instance.Hide();
            HideText();
            
            NightStatusChecks();
            if (gameEnded)
                yield break;

            string nightText = NightChecks();
            //text message
            Debug.Log(nightText);
            nightPanel.Show(nightText, moneyUpdateTotal, fearUpdateTotal, fameUpdateTotal);

            yield return new WaitForSeconds(nightDelay/2);
            
            //deck update
            cardDeck.NewDayPool(currentDay);
            cardDeck.DealCards(cardsDealtAtNight);
            currentDay++;
            cardsDrawnToday = 0;
            
            //status update
            money.Add(moneyUpdateTotal);
            moneyUpdateTotal = 0;
            fear.Add(fearUpdateTotal);
            fearUpdateTotal = 0;
            fame.Add(fameUpdateTotal);
            fameUpdateTotal = 0;
            
            yield return new WaitForSeconds(nightDelay/2);
            
            nightPanel.Hide();
            Witch.instance.Wake();
            //in case the player had put some ingredients in the pot during the night - clear the pot
            Cauldron.instance.Clear();
            DrawCard();
        }
        
        

    }
}