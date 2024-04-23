using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class ESC_InputManager : MonoBehaviour
    {
        private RecipeBook recipeBook;

        [Inject]
        private void Construct(RecipeBook recipeBook)
        {
            this.recipeBook = recipeBook;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (recipeBook.IsOpen)
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