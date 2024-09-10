using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class GameplayInstaller : MonoInstaller
    {
        [Header("Data Providers")]
        [SerializeField] private NightEventProvider nightEvents;
        [SerializeField] private EncounterDeck encounterDeck;
        [SerializeField] private IngredientsData ingredientsData;
        [SerializeField] private PriorityLaneProvider priorityLane;

        [Header("Gameplay")]
        [SerializeField] private RecipeBook recipeBook;
        [SerializeField] private ExperimentController experimentController;
        [SerializeField] private Cauldron theCauldron;
        [SerializeField] private VisitorManager visitorManager;
        [SerializeField] private GameStateMachine stateMachine;
        [SerializeField] private GameDataHandler gameDataHandler;
        [SerializeField] private CatTipsValidator catTipsValidator;

        [Header("UI")]
        [SerializeField] private GameFXManager fxManager;
        [SerializeField] private NightPanel nightPanel;
        [SerializeField] private CatTipsView catTipsView;
        [SerializeField] private CatAnimations catAnimations;

        [Inject] private MainSettings mainSettings;
        [Inject] private DataPersistenceManager dataPersistenceManager;
        [Inject] private SODictionary soDictionary;
        [Inject] private RecipeProvider recipeProvider;
        [Inject] private EndingsProvider endings;
        [Inject] private PlayerProgressProvider progressProvider;
        [Inject] private MilestoneProvider milestoneProvider;
        [Inject] private VisitorsProvider visitorProvider;

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
            Container.Bind<NightEventProvider>().FromInstance(nightEvents).AsSingle();
            Container.Bind<PriorityLaneProvider>().FromInstance(priorityLane).AsSingle();
        }

        private void BindUI()
        {
            Container.Bind<NightPanel>().FromInstance(nightPanel).AsSingle();
            Container.Bind<GameFXManager>().FromInstance(fxManager).AsSingle();
            Container.Bind<CatTipsView>().FromInstance(catTipsView).AsSingle();
            Container.Bind<CatAnimations>().FromInstance(catAnimations).AsSingle();
        }

        private void BindGameplay()
        {
            achievementManager = new AchievementManager();
            
            Container.Bind<StatusChecker>().FromNew().AsSingle();
            Container.Bind<GameStateMachine>().FromInstance(stateMachine).AsSingle().NonLazy();
            Container.Bind<StateFactory>().AsTransient();
            Container.Bind<RecipeBook>().FromInstance(recipeBook).AsSingle().NonLazy();
            Container.Bind<ExperimentController>().FromInstance(experimentController).AsSingle().NonLazy();
            Container.Bind<Cauldron>().FromInstance(theCauldron).AsSingle();
            Container.Bind<VisitorManager>().FromInstance(visitorManager).AsSingle();
            Container.Bind<CatTipsValidator>().FromInstance(catTipsValidator).AsSingle();
            Container.Bind<TooltipManager>().AsSingle().NonLazy();
            Container.Bind<IAchievementManager>().FromInstance(achievementManager).AsSingle();
            Container.Bind<GameDataHandler>().FromInstance(gameDataHandler).AsSingle().NonLazy();
        }

        private void Initialize()
        {
            gameDataHandler.Init(mainSettings, encounterDeck, dataPersistenceManager, soDictionary, progressProvider);
            encounterDeck.Init(gameDataHandler, dataPersistenceManager, soDictionary, mainSettings,
                            recipeProvider, recipeBook, milestoneProvider, progressProvider);
            nightEvents.Init(dataPersistenceManager, soDictionary, progressProvider);
            priorityLane.Init(encounterDeck, soDictionary, dataPersistenceManager, gameDataHandler, recipeBook);
            endings.Init(achievementManager);
            visitorProvider.Init(achievementManager);
        }

    }
}