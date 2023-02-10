using CauldronCodebase.GameStates;
using Save;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class GameplayInstaller : MonoInstaller
    {
        //TODO: separate installers
        //That looks nicer but it's not exactly what I meant)
        [Header("Data Providers")]
        
        [SerializeField] private RecipeProvider recipeProvider;
        [SerializeField] private NightEventProvider nightEvents;
        [SerializeField] private EncounterDeckBase encounterDeck;
        [SerializeField] private IngredientsData ingredientsData;
        [SerializeField] private EndingsProvider endings;

        [Header("Gameplay")]
        [SerializeField] private RecipeBook recipeBook;
        [SerializeField] private Cauldron theCauldron;
        [SerializeField] private VisitorManager visitorManager;
        [SerializeField] private GameStateMachine stateMachine;
        [SerializeField] private GameDataHandler gameDataHandler;

        [Header("UI")]
        [SerializeField] private EndingScreen endingScreen;

        [SerializeField] private NightPanel nightPanel;

        [Inject] private MainSettings mainSettings;
        [Inject] private DataPersistenceManager dataPersistenceManager;

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
            
            Container.Bind<EncounterDeckBase>().FromInstance(encounterDeck).AsSingle().NonLazy();
            Container.Bind<RecipeProvider>().FromInstance(recipeProvider).AsSingle();
            Container.Bind<NightEventProvider>().FromInstance(nightEvents).AsSingle();
            Container.Bind<EndingsProvider>().FromInstance(endings).AsSingle();
        }

        private void BindUI()
        {
            Container.Bind<EndingScreen>().FromInstance(endingScreen).AsSingle();
            Container.Bind<NightPanel>().FromInstance(nightPanel).AsSingle();
        }

        private void BindGameplay()
        {
            Container.Bind<GameStateMachine>().FromInstance(stateMachine).AsSingle().NonLazy();
            Container.Bind<StateFactory>().AsTransient();
            Container.Bind<RecipeBook>().FromInstance(recipeBook).AsSingle();
            Container.Bind<Cauldron>().FromInstance(theCauldron).AsSingle();
            Container.Bind<VisitorManager>().FromInstance(visitorManager).AsSingle();
            Container.Bind<TooltipManager>().AsSingle().NonLazy();
            Container.Bind<GameDataHandler>().FromInstance(gameDataHandler).AsSingle().NonLazy();
        }

        private void Initialize()
        {
            endings.Init();
        }
    }
}