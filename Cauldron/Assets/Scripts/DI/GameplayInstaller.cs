using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private MainSettings settings;
        [SerializeField] private GameManager gameLoop;
        [SerializeField] private RecipeSet recipeProvider;
        [SerializeField] private RecipeBook recipeBook;
        [SerializeField] private Cauldron theCauldron;
        [SerializeField] private VisitorManager visitorManager;
        [SerializeField] private IngredientsData ingredientsData;


        public override void InstallBindings()
        {
            //these are SOs
            Container.Bind<IngredientsData>().FromInstance(ingredientsData).AsSingle();
            Container.Bind<MainSettings>().FromInstance(settings).AsSingle().NonLazy();
            //this can be made from a prefab
            Container.Bind<GameManager>().FromInstance(gameLoop).AsSingle();
            //this can be a scriptable object
            Container.Bind<RecipeSet>().FromInstance(recipeProvider).AsSingle();
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