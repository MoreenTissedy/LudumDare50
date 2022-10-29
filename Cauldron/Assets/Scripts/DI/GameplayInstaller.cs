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


        public override void InstallBindings()
        {
            //these are SOs
            Container.Bind<IngredientsData>().FromInstance(ingredientsData).AsSingle();
            Container.Bind<MainSettings>().FromInstance(settings).AsSingle().NonLazy();
            //this can be made from a prefab
            Container.Bind<GameManager>().FromInstance(gameLoop).AsSingle();
            //these are scriptable objects
            Container.Bind<RecipeProvider>().FromInstance(recipeProvider).AsSingle();
            Container.Bind<NightEventProvider>().FromInstance(nightEvents).AsSingle();
            Container.Bind<EndingsProvider>().FromInstance(endings).AsSingle();
            endings.Init();
            //test
            endings.Unlock(EndingsProvider.Unlocks.HighFear);
            endings.Unlock(4);
            
            //this can be made from a prefab
            Container.Bind<RecipeBook>().FromInstance(recipeBook).AsSingle();
            //these are scene objects
            Container.Bind<Cauldron>().FromInstance(theCauldron).AsSingle();
            //this one can be made a prefab together with all visitors
            Container.Bind<VisitorManager>().FromInstance(visitorManager).AsSingle();
            //this one is a C# script
            Container.Bind<TooltipManager>().AsSingle().NonLazy();
        }
    }
}