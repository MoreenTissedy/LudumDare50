using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using EasyLoc;
using UnityEngine.SceneManagement;
using Zenject;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    public class GameManager : MonoBehaviour
    {

        public EncounterDeck cardDeck;
        public GameState gState;

        //TODO: reactive UI
        public NightPanel nightPanel;
        public EndingScreen endingPanel;
        public GameObject pauseMenu;
        public Canvas textCanvas;
        public VisitorTextBox visitorText;
        
        private int moneyUpdateTotal, fameUpdateTotal, fearUpdateTotal;

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
        
        

        public bool gameEnded;

        public event Action<int> NewDay;
        public event Action<int, int> NewEncounter;

        private RecipeBook recipeBook;
        private Cauldron cauldron;
        private VisitorManager visitors;
        private MainSettings settings;

        [Inject]
        public void Construct(RecipeBook book, Cauldron pot, VisitorManager vm, MainSettings settings)
        {
            recipeBook = book;
            cauldron = pot;
            visitors = vm;
            this.settings = settings;
            
            gState = new GameState(settings.gameplay.statusBarsMax, settings.gameplay.statusBarsStart);
            
            HideText();
            pauseMenu.SetActive(false);
        }

        

        private void Update()
        {
            //move to some sort of input manager?
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                if (recipeBook.bookObject.activeInHierarchy)
                {
                    recipeBook.CloseBook();
                }
                else
                {
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
            cardDeck.Init();
            CatTutorial catTutorial = GetComponent<CatTutorial>();
            if (catTutorial is null)
            {
                StartCoroutine(DrawCardWithDelay());
            }
            else 
            {
                catTutorial.Start();
                catTutorial.OnEnd += StartGame;
            }
        }
        
        

        private void StartGame()
        {
            
            CatTutorial catTutorial = GetComponent<CatTutorial>();
            if (!(catTutorial is null))
            {
                catTutorial.OnEnd -= StartGame;
            }

            DrawCard();
        }

        public void DrawCard()
        {
            Encounter currentCard = cardDeck.GetTopCard();
            gState.currentCard = currentCard;
            //in case we run out of cards
            if (gState.currentCard is null)
            {
                endingPanel.Show(endings[0]);
                EndGame();
                return;
            }
            NewEncounter?.Invoke(gState.cardsDrawnToday, settings.gameplay.cardsPerDay);
            
            currentCard.Init(this);
            gState.cardsDrawnToday ++;
            Debug.Log(currentCard.text);
            ShowText(currentCard);
            visitors.Enter(currentCard.actualVillager);
            cauldron.PotionBrewed += EndEncounter;
        }

        public void EndEncounter(Potions potion)
        {
            cauldron.PotionBrewed -= EndEncounter;
            //ChangeVisitor.instance.Exit();
            visitors.Exit();
            HideText();
            
            gState.potionsTotal.Add(potion);

            if (gState.currentCard.EndEncounter(potion))
            {
            }
            else
            {
                gState.wrongPotionsCount++;
            }
            
            //status check
            StatusChecks();
            
            if (gameEnded)
            {
                return;
            }
            
            //draw new card
            if (gState.cardsDrawnToday >= settings.gameplay.cardsPerDay)
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
            yield return new WaitForSeconds(settings.gameplay.villagerDelay);
            DrawCard();
        }

        NightEvent GetRandomEvent()
        {
            if (gState.events.Count == 0)
                return null;
            int i = Random.Range(0, gState.events.Count);
            NightEvent randomEvent = gState.events[i];
            gState.events.Remove(randomEvent);
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
                    if (gState.potionsTotal.Count(x => x == condition.type) < condition.threshold)
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
            if (gState.Fame >= settings.gameplay.statusBarsMax)
            {
                endingPanel.Show(endings[1]);
                endingReached = true;
            }
            else if (gState.Fear >= settings.gameplay.statusBarsMax)
            {
                endingPanel.Show(endings[2]);
                endingReached = true;
            }
            else if (gState.Money >= settings.gameplay.statusBarsMax)
            {
                endingPanel.Show(endings[0]);
                endingReached = true;

            }
            else if (gState.Fame <= 0)
            {
                endingPanel.Show(endings[3]);
                endingReached = true;
            }
            else if (gState.Fear <= 0)
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
            if (gState.Fame > threshold)
            {
                //add first index
                cardDeck.AddToDeck(Encounter.GetRandom(highFameCards));
            }

            if (gState.Fear > threshold)
            {
                //add first index
                cardDeck.AddToDeck(Encounter.GetRandom(highFearCards));
            }
        }

        void WrongPotionCheck(ref string text)
        {
            if (!check2passed && gState.wrongPotionsCount >= wrongPotionsCheck2)
            {
                check2passed = true;
                fameUpdateTotal += wrongPotionsFameMod2;
                text += wrongPotionCheckText2;
            }
            else if (!check1passed && gState.wrongPotionsCount >= wrongPotionsCheck1)
            {
                check1passed = true;
                fameUpdateTotal += wrongPotionsFameMod1;
                text += wrongPotionCheckText1;
            }
        }
        
        private IEnumerator StartNewDay()
        {
            yield return new WaitForSeconds(settings.gameplay.villagerDelay);
            
            //Witch.instance.Hide();
            HideText();
            NewDay?.Invoke(gState.currentDay+1);
            
            StatusChecks();
            if (gameEnded)
                yield break;

            string nightText = NightChecks();
            //text message
            Debug.Log(nightText);
            nightPanel.Show(nightText, moneyUpdateTotal, fearUpdateTotal, fameUpdateTotal);
            
            yield return new WaitForSeconds(settings.gameplay.nightDelay/2);
            
            //deck update
            cardDeck.NewDayPool(gState.currentDay);
            cardDeck.DealCards(settings.gameplay.cardsDealtAtNight);
            gState.currentDay++;
            gState.cardsDrawnToday = 0;
            
            //status update
            gState.Money += moneyUpdateTotal;
            moneyUpdateTotal = 0;
            gState.Fear += fearUpdateTotal;
            fearUpdateTotal = 0;
            gState.Fame += fameUpdateTotal;
            fameUpdateTotal = 0;
            
            
            //yield return new WaitForSeconds(nightDelay/2);
            yield return new WaitUntil(() => Input.anyKeyDown);

            nightPanel.Hide();
            //Witch.instance.Wake();
            
            StatusChecks();
            if (gameEnded)
                yield break;
            
            DrawCard();
        }
        
        

    }
}