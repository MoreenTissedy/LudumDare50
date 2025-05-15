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
        private OverlayManager overlayManager;

        [Inject]
        private void Construct(RecipeBook recipeBook, InputManager inputManager, Wardrobe wardrobe, OverlayManager overlayManager)
        {
            this.recipeBook = recipeBook;
            this.wardrobe = wardrobe;
            recipeBookModeTotal = Enum.GetValues(typeof(RecipeBook.Mode)).Length;
            
            controls = inputManager.Controls;
            this.overlayManager = overlayManager;
            
            controls.General.Exit.performed += ProcessExit;
            controls.General.BookToggle.performed += ToggleBook;
            controls.General.BookNavigate.performed += BookNavigateUpDown;
        }

        private void ToggleBook(InputAction.CallbackContext input)
        {
            if (recipeBook.isNightBook) return;
            if (overlayManager.GetCurrentLayer == Layers.Base || overlayManager.GetCurrentLayer == Layers.RecipeBook)
            {
                recipeBook.ToggleBook();
            }
        }
        
        private void BookNavigateUpDown(InputAction.CallbackContext input)
        {
            if (overlayManager.GetCurrentLayer != Layers.RecipeBook)
            {
                return;
            }
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
            
            if (overlayManager.GetCurrentLayer == Layers.RecipeBook)
            {
                recipeBook.CloseBook();
            }
            else if (overlayManager.GetCurrentLayer == Layers.RecipeBook)
            {
                wardrobe.CloseWithoutApply();
            }
            else if (overlayManager.GetCurrentLayer == Layers.Base)
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