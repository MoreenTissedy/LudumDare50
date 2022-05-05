using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using EasyLoc;
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
        public GameObject pauseMenu;
        public Canvas textCanvas;
        public VisitorTextBox visitorText;
        
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
        [Localize]
        public string defaultNightText1 = "Ничего необычного.";
        [Localize]
        public string defaultNightText2 = "Ночь спокойна и тиха.";
        [Localize]
        public string defaultNightText3 = "Дует ветер, гонит тучки.";
        

        [Space(5)] 
        public int wrongPotionsCheck1 = 5;
        public int wrongPotionsFameMod1 = -20;
        public int wrongPotionsCheck2 = 10;
        public int wrongPotionsFameMod2 = -50;
        private bool check1passed, check2passed;
        [Localize] [TextArea(3, 7)]
        public string wrongPotionCheckText1 = "Люди фыркают и поджимают губы, когда слышат ваше имя — " +
                                              "они недовольны тем, что ваши зелья им не помогают. ";
        [Localize] [TextArea(3, 7)]
        public string wrongPotionCheckText2 = "Крестьяне презрительно отзываются о ваших способностях: " +
                                              "«Да она ни одного зелья правильно сварить не может!» ";
        [Space(5)]

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
        public int wrongPotionsCount;
        public int rightPotionsCount;
        public List<NightEvent> events;

        public bool gameEnded;

        public event Action<int> NewDay;
        public event Action<int, int> NewEncounter;
        
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
            events = new List<NightEvent>(5);
            
            money = new Status(Statustype.Money);
            fame = new Status(Statustype.Fame);
            fear = new Status(Statustype.Fear);
            
            HideText();
            pauseMenu.SetActive(false);
        }

        public Status GetStatusByType(Statustype type)
        {
            switch (type)
            {
                case Statustype.Money:
                    return money;
                case Statustype.Fear:
                    return fear;
                case Statustype.Fame:
                    return fame;
            }

            return null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                if (RecipeBook.instance.bookObject.activeInHierarchy)
                {
                    Debug.Log("recipe book closed");
                    RecipeBook.instance.CloseBook();
                }
                else
                {
                    Debug.Log("pause menu");
                    pauseMenu.SetActive(true);
                }
            }
        }

        public void Continue()
        {
            pauseMenu.SetActive(false);
        }

        public void ShowText(Encounter card)
        {
            textCanvas.gameObject.SetActive(true);
            visitorText.Display(card);
        }

        public void HideText()
        {
            textCanvas.gameObject.SetActive(false);
        }

        private void Start()
        {
            Debug.Log("start GM");
            cardDeck.Init();
            //Delay draw card to ensure every object has initialized
            Invoke("DrawCard",0.2f);
        }

        public void DrawCard()
        {
            NewEncounter?.Invoke(cardsDrawnToday, cardsPerDay);
            Cauldron.instance.Clear();
            currentCard = cardDeck.GetTopCard();
            if (currentCard == null)
            {
                endingPanel.Show(endings[0]);
                EndGame();
                return;
            }
            currentCard.Init();
            cardsDrawnToday ++;
            Debug.Log(currentCard.text);
            ShowText(currentCard);
            //ChangeVisitor.instance.Enter(currentCard.actualVillager);
            //VisitorManager.instance.EnterDefault();
            VisitorManager.instance.Enter(currentCard.actualVillager);
        }

        public void EndEncounter(Potions potion)
        {
            //ChangeVisitor.instance.Exit();
            VisitorManager.instance.Exit();
            HideText();
            
            potionPopup.Show(RecipeBook.instance.GetRecipeForPotion(potion));
            potionsTotal.Add(potion);

            if (currentCard.EndEncounter(potion))
            {
                rightPotionsCount++;
            }
            else
            {
                wrongPotionsCount++;
            }
            
            //status check
            StatusChecks();
            
            if (gameEnded)
            {
                potionPopup.Hide();
                return;
            }
            
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

        NightEvent GetRandomEvent()
        {
            if (events.Count == 0)
                return null;
            int i = Random.Range(0, events.Count);
            NightEvent randomEvent = events[i];
            events.Remove(randomEvent);
            return randomEvent;
        }
        string NightChecks()
        {
            string text = String.Empty;

            var nightEvent = GetRandomEvent();
            if (nightEvent != null)
            {
                text = nightEvent.flavourText;
                fearUpdateTotal += nightEvent.fearModifier;
                fameUpdateTotal += nightEvent.fameModifier;
                moneyUpdateTotal += nightEvent.moneyModifier;
            }
            else
            {
                List<NightCondition> toRemove = new List<NightCondition>(3);
                foreach (var condition in nightConditions)
                {
                    if (potionsTotal.Count(x => x == condition.type) < condition.threshold)
                        continue;
                    text = condition.flavourText + " ";
                    moneyUpdateTotal += condition.moneyModifier;
                    fearUpdateTotal += condition.fearModifier;
                    fameUpdateTotal += condition.fameModifier;
                    if (condition.bonusCard != null)
                        cardDeck.AddCardToPool(condition.bonusCard);
                    toRemove.Add(condition);
                    //one condition per night
                    break;
                }

                foreach (var condition in toRemove)
                {
                    nightConditions.Remove(condition);
                }
            }
            
            if (text == String.Empty)
                WrongPotionCheck(ref text);

            if (text == String.Empty)
            {
                int rnd = Random.Range(0, 3);
                switch (rnd)
                {
                    case 0:
                        text = defaultNightText1;
                        break;
                    case 1:
                        text = defaultNightText2;
                        break;
                    case 2:
                        text = defaultNightText3;
                        break;
                }
            }

            return text;
        }


        IEnumerator EndGame()
        {
            gameEnded = true;
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => Input.anyKeyDown);
            ReloadGame();
        }
        
        [ContextMenu("Export Night Conditions to CSV")]
        public void ExportConditions()
        {
            var file = File.CreateText(Application.dataPath+"/Localize/Night_events.csv");
            file.WriteLine("id;flavour_RU;flavour_EN");
            foreach (var condition in nightConditions)
            {
                file.WriteLine(condition.name+";"+condition.flavourText);
            }
            file.Close();
        }
        
        [ContextMenu("Export Endings to CSV")]
        public void ExportEndings()
        {
            var file = File.CreateText(Application.dataPath+"/Localize/Endings.csv");
            file.WriteLine("id;description_RU;description_EN");
            foreach (var ending in endings)
            {
                file.WriteLine(ending.name+";"+ending.text);
            }
            file.Close();
        }

        public void ReloadGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        public void Exit()
        {
            Application.Quit();
        }
        
        
        void StatusChecks()
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
                //add first index
                cardDeck.AddToDeck(Encounter.GetRandom(highFameCards));
            }

            if (fear.Value() > threshold)
            {
                //add first index
                cardDeck.AddToDeck(Encounter.GetRandom(highFearCards));
            }
        }

        void WrongPotionCheck(ref string text)
        {
            if (!check2passed && wrongPotionsCount >= wrongPotionsCheck2)
            {
                check2passed = true;
                fameUpdateTotal += wrongPotionsFameMod2;
                text += wrongPotionCheckText2;
            }
            else if (!check1passed && wrongPotionsCount >= wrongPotionsCheck1)
            {
                check1passed = true;
                fameUpdateTotal += wrongPotionsFameMod1;
                text += wrongPotionCheckText1;
            }
        }
        
        private IEnumerator StartNewDay()
        {
            yield return new WaitForSeconds(villagerDelay);
            
            potionPopup.Hide();
            Witch.instance.Hide();
            HideText();
            NewDay?.Invoke(currentDay+1);
            
            StatusChecks();
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
            
            
            //yield return new WaitForSeconds(nightDelay/2);
            yield return new WaitUntil(() => Input.anyKeyDown);

            nightPanel.Hide();
            Witch.instance.Wake();
            //in case the player had put some ingredients in the pot during the night - clear the pot
            Cauldron.instance.Clear();
            
            StatusChecks();
            if (gameEnded)
                yield break;
            
            DrawCard();
        }
        
        

    }
}