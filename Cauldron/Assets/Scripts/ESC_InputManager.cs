using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class ESC_InputManager : MonoBehaviour
    {
        private RecipeBook recipeBook;
        private Tutorial tutorial;

        [Inject]
        private void Construct(RecipeBook recipeBook, Tutorial tutorial)
        {
            this.recipeBook = recipeBook;
            this.tutorial = tutorial;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (tutorial.IsVisible)
                {
                    tutorial.CloseAllPages();
                }
                else if (recipeBook.IsOpen)
                {
                    recipeBook.CloseBook();
                }
                else if (!GameLoader.IsMenuOpen())
                {
                    GameLoader.LoadMenu();
                }
            }
        }
    }
}