using CauldronCodebase;
using CauldronCodebase.GameStates;
using UnityEngine;
using UnityEngine.InputSystem;
using Universal;
using Zenject;

namespace UI.Buttons
{
    public class TooltipGamepadMediator : MonoBehaviour
    {
        public ScrollTooltip tooltip;
        private float lastInputTime;

        [Inject] private InputManager inputManager;
        [Inject] private OverlayManager overlayManager;
        [Inject] private GameStateMachine gameStateMachine;

        public void Reset()
        {
            tooltip = GetComponent<ScrollTooltip>();
        }

        private void OnEnable()
        {
            lastInputTime = Time.realtimeSinceStartup + 0.2f;
            gameStateMachine.OnChangeState += CloseTooltipsOnNewDay;
            inputManager.Controls.General.ToggleTooltips.performed += HandleTooltipToggle;
        }

        private void OnDisable()
        {
            gameStateMachine.OnChangeState -= CloseTooltipsOnNewDay;
            inputManager.Controls.General.ToggleTooltips.performed -= HandleTooltipToggle;
        }

        private void CloseTooltipsOnNewDay(GameStateMachine.GamePhase _)
        {
            tooltip.Close();
        }

        private void HandleTooltipToggle(InputAction.CallbackContext context)
        {
            if (overlayManager.GetCurrentLayer != Layers.Base)
            {
                return;
            }

            if (Time.realtimeSinceStartup - lastInputTime < 0.3f)
            {
                return;
            }

            lastInputTime = Time.realtimeSinceStartup;
            tooltip.Toggle();
        }
    }
}