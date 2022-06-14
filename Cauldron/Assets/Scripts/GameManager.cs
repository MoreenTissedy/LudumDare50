using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using Zenject;

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
  
        
        [Space(5)]

        [Tooltip("High money, high fame, high fear, low fame, low fear")]
        public Ending[] endings;
        public int threshold = 70;
        public Encounter[] highFameCards, highFearCards;

        [Tooltip("A list of conditions for total potions brewed checks that happen at night.")]
        public List<TotalPotionCountEvent> nightConditions;
        
        

        public bool gameEnded;

        public event Action<int> NewDay;
        public event Action<int, int> NewEncounter;

        private RecipeBook recipeBook;
        private Cauldron cauldron;
        private VisitorManager visitors;
        private MainSettings settings;
        public NightEventProvider nightEvents;

        [Inject]
        public void Construct(RecipeBook book, Cauldron pot, VisitorManager vm, MainSettings settings, NightEventProvider nightEventProvider)
        {
            recipeBook = book;
            cauldron = pot;
            visitors = vm;
            this.settings = settings;
            nightEvents = nightEventProvider;
            
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

        private IEnumerator StartNewDay()
        {
            yield return new WaitForSeconds(settings.gameplay.villagerDelay);

            //Witch.instance.Hide();
            HideText();
            NewDay?.Invoke(gState.currentDay + 1);

            StatusChecks();
            if (gameEnded)
                yield break;

            //night events
            nightPanel.Show(nightEvents.GetEvents(gState));

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