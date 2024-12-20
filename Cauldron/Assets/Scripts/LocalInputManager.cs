using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace CauldronCodebase
{
    public class LocalInputManager : MonoBehaviour
    {
        private RecipeBook recipeBook;
        private Wardrobe wardrobe;
        private int recipeBookModeTotal;
        private Controls controls;

        [Inject]
        private void Construct(RecipeBook recipeBook, InputManager inputManager, Wardrobe wardrobe)
        {
            this.recipeBook = recipeBook;
            this.wardrobe = wardrobe;
            recipeBookModeTotal = Enum.GetValues(typeof(RecipeBook.Mode)).Length;
            
            controls = inputManager.Controls;
            
            controls.General.Exit.performed += ProcessExit;
            controls.General.BookToggle.performed += ToggleBook;
            controls.General.BookNavigate.performed += BookNavigateUpDown;
        }

        private void ToggleBook(InputAction.CallbackContext input)
        {
            if (recipeBook.isNightBook) return;
            
            recipeBook.ToggleBook();
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
            if (GameLoader.IsMenuOpen())
            {
                return;
            }
            
            if (recipeBook.IsOpen)
            {
                recipeBook.CloseBook();
            }
            else if (wardrobe.IsOpen)
            {
                wardrobe.CloseWithoutApply();
            }
            else
            {
                GameLoader.LoadMenu();
            }
        }

        private void OnDestroy()
        {
            controls.General.Exit.performed -= ProcessExit;
            controls.General.BookToggle.performed -= ToggleBook;
            controls.General.BookNavigate.performed -= BookNavigateUpDown;
        }
    }
}