using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameManager gameLoop;
        [SerializeField] private RecipeSet recipeProvider;
        [SerializeField] private RecipeBook recipeBook;
        [SerializeField] private Cauldron theCauldron;
        [SerializeField] private Witch witch;
        [SerializeField] private VisitorManager visitorManager;

        public override void InstallBindings()
        {
            //this can be made from a prefab
            Container.Bind<GameManager>().FromInstance(gameLoop).AsSingle();
            //this can be a scriptable object
            Container.Bind<RecipeSet>().FromInstance(recipeProvider).AsSingle();
            //this can be made from a prefab
            Container.Bind<RecipeBook>().FromInstance(recipeBook).AsSingle();
            //these are scene objects
            Container.Bind<Cauldron>().FromInstance(theCauldron).AsSingle();
            Container.Bind<Witch>().FromInstance(witch).AsSingle();
            //this one can be made a prefab together with all visitors
            Container.Bind<VisitorManager>().FromInstance(visitorManager).AsSingle();
            //this one is a C# script
            Container.Bind<TooltipManager>().AsSingle().NonLazy();
        }
    }
}