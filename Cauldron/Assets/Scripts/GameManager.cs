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
        [SerializeField] private EncounterDeck cardDeck;
        private GameState gState;
        private NightEventProvider nightEvents;
        public EncounterDeck CardDeck => cardDeck;
        public GameState GameState => gState;
        public NightEventProvider NightEvents => nightEvents;

        //TODO: reactive UI
        public NightPanel nightPanel;
        public EndingScreen endingPanel;
        public GameObject pauseMenu;
        
        
        [SerializeField] private bool gameEnded;
        public bool GameEnded => gameEnded;

        public event Action<int> NewDay;
        public event Action<int, int> NewEncounter;

        private RecipeBook recipeBook;
        private Cauldron cauldron;
        private VisitorManager visitors;
        private MainSettings settings;

        public MainSettings Settings => settings;

        private EndingsProvider endings;

        [Inject]
        public void Construct(RecipeBook book,
            Cauldron pot,
            VisitorManager vm,
            MainSettings settings,
            NightEventProvider nightEventProvider,
            EndingsProvider endingsProvider)
        {
            recipeBook = book;
            cauldron = pot;
            visitors = vm;
            this.settings = settings;
            nightEvents = nightEventProvider;
            endings = endingsProvider;
            
            gState = new GameState(settings.gameplay.statusBarsMax, settings.gameplay.statusBarsStart);
            
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
                EndGameProcess(endings.endings[0]);
                return;
            }
            NewEncounter?.Invoke(gState.cardsDrawnToday, Settings.gameplay.cardsPerDay);
            
            currentCard.Init(this);
            gState.cardsDrawnToday ++;
            Debug.Log(currentCard.text);
            visitors.Enter(currentCard);
            cauldron.PotionBrewed += EndEncounter;
        }

        public void EndEncounter(Potions potion)
        {
            cauldron.PotionBrewed -= EndEncounter;
            visitors.Exit();
            
            gState.potionsTotal.Add(potion);

            if (!gState.currentCard.EndEncounter(potion))
            {
                gState.wrongPotionsCount++;
            }

            //status check
            endings.StatusChecks(this);
            
            if (gameEnded)
            {
                return;
            }
            
            if (gState.cardsDrawnToday >= Settings.gameplay.cardsPerDay)
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
            yield return new WaitForSeconds(Settings.gameplay.villagerDelay);
            DrawCard();
        }

        public void EndGame(Ending ending)
        {
            StartCoroutine(EndGameProcess(ending));
        }
        
        IEnumerator EndGameProcess(Ending ending)
        {
            endingPanel.Show(ending);
            gameEnded = true;
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => Input.anyKeyDown);
            ReloadGame();
        }
        
       
        public void ReloadGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        public void Exit()
        {
            Application.Quit();
        }
        
       
        private IEnumerator StartNewDay()
        {
            yield return new WaitForSeconds(Settings.gameplay.villagerDelay);

            //Witch.instance.Hide();
            NewDay?.Invoke(gState.currentDay + 1);

            endings.StatusChecks(this);
            if (gameEnded)
                yield break;

            //night events
            var events = nightEvents.GetEvents(gState);
            nightPanel.Show(events);
            foreach (NightEvent nightEvent in events)
            {
                nightEvent.ApplyModifiers(gState);
            }

            yield return new WaitForSeconds(Settings.gameplay.nightDelay/2);
            
            //deck update
            cardDeck.NewDayPool(gState.currentDay);
            cardDeck.DealCards(Settings.gameplay.cardsDealtAtNight);
            gState.currentDay++;
            gState.cardsDrawnToday = 0;
            
            
            //yield return new WaitForSeconds(nightDelay/2);
            yield return new WaitUntil(() => Input.anyKeyDown);

            nightPanel.Hide();
            //Witch.instance.Wake();
            
            endings.StatusChecks(this);
            if (gameEnded)
                yield break;
            
            DrawCard();
        }
        
        

    }
}