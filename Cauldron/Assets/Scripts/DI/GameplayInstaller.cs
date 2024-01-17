using CauldronCodebase.GameStates;
using Save;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class GameplayInstaller : MonoInstaller
    {
        [Header("Data Providers")]
        [SerializeField] private RecipeProvider recipeProvider;
        [SerializeField] private NightEventProvider nightEvents;
        [SerializeField] private EncounterDeck encounterDeck;
        [SerializeField] private IngredientsData ingredientsData;
        [SerializeField] private EndingsProvider endings;
        [SerializeField] private PriorityLaneProvider priorityLane;

        [Header("Gameplay")]
        [SerializeField] private RecipeBook recipeBook;
        [SerializeField] private Cauldron theCauldron;
        [SerializeField] private VisitorManager visitorManager;
        [SerializeField] private GameStateMachine stateMachine;
        [SerializeField] private GameDataHandler gameDataHandler;
        [SerializeField] private CatTipsValidator catTipsValidator;

        [Header("UI")]
        [SerializeField] private GameFXManager fxManager;
        [SerializeField] private NightPanel nightPanel;
        [SerializeField] private CatTipsView catTipsView;

        [Inject] private MainSettings mainSettings;
        [Inject] private DataPersistenceManager dataPersistenceManager;
        [Inject] private SODictionary soDictionary;

        private IAchievementManager achievementManager;

        public override void InstallBindings()
        {
            BindDataProviders();
            BindGameplay();
            BindUI();
            Initialize();
        }

        private void BindDataProviders()
        {
            Container.Bind<IngredientsData>().FromInstance(ingredientsData).AsSingle();
            Container.Bind<EncounterDeck>().FromInstance(encounterDeck).AsSingle().NonLazy();
            Container.Bind<RecipeProvider>().FromInstance(recipeProvider).AsSingle();
            Container.Bind<NightEventProvider>().FromInstance(nightEvents).AsSingle();
            Container.Bind<EndingsProvider>().FromInstance(endings).AsSingle();
            Container.Bind<PriorityLaneProvider>().FromInstance(priorityLane).AsSingle();
        }

        private void BindUI()
        {
            Container.Bind<NightPanel>().FromInstance(nightPanel).AsSingle();
            Container.Bind<GameFXManager>().FromInstance(fxManager).AsSingle();
            Container.Bind<CatTipsView>().FromInstance(catTipsView).AsSingle();
        }

        private void BindGameplay()
        {
            achievementManager = new AchievementManager();
            
            Container.Bind<StatusChecker>().FromNew().AsSingle();
            Container.Bind<GameStateMachine>().FromInstance(stateMachine).AsSingle().NonLazy();
            Container.Bind<StateFactory>().AsTransient();
            Container.Bind<RecipeBook>().FromInstance(recipeBook).AsSingle().NonLazy();
            Container.Bind<Cauldron>().FromInstance(theCauldron).AsSingle();
            Container.Bind<VisitorManager>().FromInstance(visitorManager).AsSingle();
            Container.Bind<CatTipsValidator>().FromInstance(catTipsValidator).AsSingle();
            Container.Bind<TooltipManager>().AsSingle().NonLazy();
            Container.Bind<IAchievementManager>().FromInstance(achievementManager).AsSingle();
            Container.Bind<GameDataHandler>().FromInstance(gameDataHandler).AsSingle().NonLazy();
        }

        private void Initialize()
        {
            SetInitialResolution();
            gameDataHandler.Init(mainSettings, encounterDeck, dataPersistenceManager, soDictionary);
            encounterDeck.Init(gameDataHandler, dataPersistenceManager, soDictionary, mainSettings, recipeProvider);
            nightEvents.Init(dataPersistenceManager, soDictionary);
            priorityLane.Init(encounterDeck, soDictionary, dataPersistenceManager, gameDataHandler);
            endings.Init(achievementManager);
        }

        private static void SetInitialResolution()
        {
            bool fullscreenMode = true;
            if (PlayerPrefs.HasKey(PrefKeys.FullscreenModeSettings))
            {
                fullscreenMode = PlayerPrefs.GetInt(PrefKeys.FullscreenModeSettings) == 1;
            }

            if (PlayerPrefs.HasKey(PrefKeys.ResolutionSettings))
            {
                var resolutions = Screen.resolutions;
                int newResolution = PlayerPrefs.GetInt(PrefKeys.ResolutionSettings);
                Screen.SetResolution(resolutions[newResolution].width, resolutions[newResolution].height, fullscreenMode);
            }
            else
            {
                Screen.SetResolution(1920, 1080, fullscreenMode);
            }
        }
    }
}