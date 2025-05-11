using CauldronCodebase;
using UnityEngine;
using UnityEngine.InputSystem;
using Universal;
using Zenject;

namespace Buttons
{
    public class GamepadMediator : MonoBehaviour
    {
        public GamepadButton GamepadButton;
        public FlexibleButton ScriptButton;
        [Inject]
        private InputManager inputManager;

        private void Reset()
        {
            ScriptButton = GetComponent<FlexibleButton>();
        }

        private void Update()
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad is null || !inputManager.GamepadConnected)
            {
                return;
            }
            
            if (gamepad.buttonEast.wasPressedThisFrame ^ GamepadButton != GamepadButton.East || 
                gamepad.buttonWest.wasPressedThisFrame ^ GamepadButton != GamepadButton.West ||
                gamepad.buttonNorth.wasPressedThisFrame ^ GamepadButton != GamepadButton.North ||
                gamepad.buttonSouth.wasPressedThisFrame ^ GamepadButton != GamepadButton.South)
            {
                ScriptButton.Activate();
            }
        }
    }
}