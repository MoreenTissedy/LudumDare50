using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private MainSettings settings;
        [SerializeField] private GameManager gameLoop;
        [SerializeField] private RecipeProvider recipeProvider;
        [SerializeField] private NightEventProvider nightEvents;
        [SerializeField] private RecipeBook recipeBook;
        [SerializeField] private Cauldron theCauldron;
        [SerializeField] private VisitorManager visitorManager;
        [SerializeField] private IngredientsData ingredientsData;
        [SerializeField] private EndingsProvider endings;
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private EncounterDeckBase encounterDeck;


        public override void InstallBindings()
        {
            Container.Bind<StateMachine>().FromInstance(stateMachine).AsSingle().NonLazy();
            
            Container.Bind<IngredientsData>().FromInstance(ingredientsData).AsSingle();
            Container.Bind<MainSettings>().FromInstance(settings).AsSingle().NonLazy();
            Container.Bind<EncounterDeckBase>().FromInstance(encounterDeck).AsSingle();
            Container.Bind<GameManager>().FromInstance(gameLoop).AsSingle();
            Container.Bind<RecipeProvider>().FromInstance(recipeProvider).AsSingle();
            Container.Bind<NightEventProvider>().FromInstance(nightEvents).AsSingle();
            Container.Bind<EndingsProvider>().FromInstance(endings).AsSingle();
            Container.Bind<RecipeBook>().FromInstance(recipeBook).AsSingle();
            Container.Bind<Cauldron>().FromInstance(theCauldron).AsSingle();
            Container.Bind<VisitorManager>().FromInstance(visitorManager).AsSingle();
            Container.Bind<TooltipManager>().AsSingle().NonLazy();

            //[Inject] attribute invokes Construct() for us, just delete this line
            stateMachine.Construct(encounterDeck,settings,visitorManager,theCauldron,nightEvents);
            
            nightEvents.Init();
            endings.Init();
        }
    }
}