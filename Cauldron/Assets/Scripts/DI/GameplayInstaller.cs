using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class GameplayInstaller : MonoInstaller
    {
        [Header("Data Providers")]
        [SerializeField] private MainSettings settings;
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

        [Header("UI")]
        [SerializeField] private EndingScreen endingScreen;
        [SerializeField] private NightPanel nightPanel;

        private GameData gameData;

        public override void InstallBindings()
        {
            BindDataProviders();
            BindGameplay();
            BindUI();
            Initialize();
        }

        private void BindDataProviders()
        {
            gameData = new GameData(settings.statusBars, encounterDeck, nightEvents);
            Container.Bind<GameData>().FromInstance(gameData).AsSingle();

            Container.Bind<IngredientsData>().FromInstance(ingredientsData).AsSingle();
            Container.Bind<MainSettings>().FromInstance(settings).AsSingle().NonLazy();
            Container.Bind<EncounterDeckBase>().FromInstance(encounterDeck).AsSingle();
            Container.Bind<RecipeProvider>().FromInstance(recipeProvider).AsSingle();
            Container.Bind<NightEventProvider>().FromInstance(nightEvents).AsSingle();
            Container.Bind<EndingsProvider>().FromInstance(endings).AsSingle();
            Container.Bind<StateFactory>().AsSingle();
        }

        private void BindUI()
        {
            Container.Bind<EndingScreen>().FromInstance(endingScreen).AsSingle();
            Container.Bind<NightPanel>().FromInstance(nightPanel).AsSingle();
        }

        private void BindGameplay()
        {
            Container.Bind<GameStateMachine>().FromInstance(stateMachine).AsSingle().NonLazy();
            Container.Bind<RecipeBook>().FromInstance(recipeBook).AsSingle();
            Container.Bind<Cauldron>().FromInstance(theCauldron).AsSingle();
            Container.Bind<VisitorManager>().FromInstance(visitorManager).AsSingle();
            Container.Bind<TooltipManager>().AsSingle().NonLazy();
        }

        private void Initialize()
        {
            nightEvents.Init();
            endings.Init();
            encounterDeck.Init(gameData);
        }
    }
}