using UnityEngine;
using UnityEngine.InputSystem;
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
            var controls = new Controls();
            controls.General.Enable();
            controls.General.Exit.performed += Process;
        }

        private void Process(InputAction.CallbackContext context)
        {
            if (recipeBook.IsOpen)
            {
                return;
            }
            else if (!GameLoader.IsMenuOpen())
            {
                GameLoader.LoadMenu();
            }
        }
    }
}