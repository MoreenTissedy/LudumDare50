using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace CauldronCodebase
{
    public class LocalInputManager : MonoBehaviour
    {
        private RecipeBook recipeBook;
        private int recipeBookModeTotal;

        [Inject]
        private void Construct(RecipeBook recipeBook, InputManager inputManager)
        {
            this.recipeBook = recipeBook;
            recipeBookModeTotal = Enum.GetValues(typeof(RecipeBook.Mode)).Length;
            
            var controls = inputManager.Controls;
            
            controls.General.Exit.performed += ProcessExit;
            controls.General.BookToggle.performed += (_) => recipeBook.ToggleBook();
            controls.General.BookNavigate.performed += BookNavigateUpDown;
        }

        private void BookNavigateUpDown(InputAction.CallbackContext input)
        {
            float upDown = input.ReadValue<Vector2>().y;
            int currentMode = (int)recipeBook.CurrentMode;
            if (upDown > 0 && currentMode > 0)
            {
                recipeBook.ChangeMode((RecipeBook.Mode)(currentMode - 1));
            }
            else if (upDown < 0 && currentMode < recipeBookModeTotal - 1)
            {
                recipeBook.ChangeMode((RecipeBook.Mode)(currentMode + 1));
            }
        }

        private void ProcessExit(InputAction.CallbackContext context)
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