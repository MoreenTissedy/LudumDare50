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
        private CatTipsView catTipsView;
        private InputManager inputManager;
        
        private float lastBookInputTime;
        private float lastHintInputTime;

        [Inject]
        private void Construct(RecipeBook recipeBook, InputManager inputManager, Wardrobe wardrobe, OverlayManager overlayManager, CatTipsView catTipsView)
        {
            this.recipeBook = recipeBook;
            this.wardrobe = wardrobe;
            this.catTipsView = catTipsView;
            recipeBookModeTotal = Enum.GetValues(typeof(RecipeBook.Mode)).Length - 1;

            this.inputManager = inputManager;
            controls = inputManager.Controls;
            this.overlayManager = overlayManager;
            
            controls.General.Exit.performed += ProcessExit;
            controls.General.BookToggle.started += ToggleBook;
            controls.General.BookNavigate.performed += BookNavigateUpDown;
        }

        private void ToggleBook(InputAction.CallbackContext input)
        {
            if (recipeBook.isNightBook) return;
            if (Time.realtimeSinceStartup - lastBookInputTime < 0.5f)
            {
                return;
            }
            lastBookInputTime = Time.realtimeSinceStartup;
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
            else if (overlayManager.GetCurrentLayer == Layers.Wardrobe)
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

        private void Update()
        {
            if (overlayManager.GetCurrentLayer != Layers.Base)
            {
                return;
            }
            Gamepad gamepad = Gamepad.current;
            if (!inputManager.GamepadConnected || gamepad is null)
            {
                return;
            }
            
            if (gamepad.buttonWest.wasPressedThisFrame)
            {
                if (Time.realtimeSinceStartup - lastHintInputTime < 0.3f)
                {
                    return;
                }
                lastHintInputTime = Time.realtimeSinceStartup;
                if (catTipsView.HasTip)
                {
                    catTipsView.ChangeTipView();
                }
            }
        }
    }
}