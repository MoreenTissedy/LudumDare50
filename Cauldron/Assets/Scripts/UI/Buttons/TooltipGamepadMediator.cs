using System;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using UnityEngine;
using UnityEngine.InputSystem;
using Universal;
using Zenject;

namespace UI.Buttons
{
    public class TooltipGamepadMediator: MonoBehaviour
    {
        public ScrollTooltip tooltip;
        public GamepadButton Button = GamepadButton.North;
        
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
            //initial delay
            lastInputTime = Time.realtimeSinceStartup + 0.2f;
            gameStateMachine.OnChangeState += CloseTooltipsOnNewDay;
        }

        private void CloseTooltipsOnNewDay(GameStateMachine.GamePhase _)
        {
            tooltip.Close();
        }

        private void OnDisable()
        {
            gameStateMachine.OnChangeState -= CloseTooltipsOnNewDay;
        }

        private void Update()
        {
            if (overlayManager.GetCurrentLayer != Layers.Base)
            {
                return;
            }
            Gamepad gamepad = Gamepad.current;
            if (gamepad is null || !inputManager.GamepadConnected)
            {
                return;
            }
            
            if (gamepad.buttonEast.wasPressedThisFrame & Button == GamepadButton.East || 
                gamepad.buttonWest.wasPressedThisFrame & Button == GamepadButton.West ||
                gamepad.buttonNorth.wasPressedThisFrame & Button == GamepadButton.North ||
                gamepad.buttonSouth.wasPressedThisFrame & Button == GamepadButton.South)
            {
                if (Time.realtimeSinceStartup - lastInputTime < 0.3f)
                {
                    return;
                }
                lastInputTime = Time.realtimeSinceStartup;
                tooltip.Toggle();
            }
        }
        
    }
}