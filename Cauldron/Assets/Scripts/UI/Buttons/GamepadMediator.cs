using CauldronCodebase;
using UnityEngine;
using UnityEngine.InputSystem;
using Universal;
using Zenject;

namespace Buttons
{
    public class GamepadMediator : MonoBehaviour, IOverlayElement
    {
        public GamepadButton GamepadButton;
        public FlexibleButton ScriptButton;
        [Inject]
        private InputManager inputManager;
        
        public GamepadButton Button 
        {
            get
            {
                GamepadButton button = GamepadButton;
                if (inputManager.GamepadType == GamepadType.Switch)
                {
                    if (GamepadButton == GamepadButton.South)
                    {
                        button = GamepadButton.East;
                    }
                    else if (GamepadButton == GamepadButton.East)
                    {
                        button = GamepadButton.South;
                    }
                }

                return button;
            }
        }
        
        private float lastInputTime;
        private bool locked;

        private void Reset()
        {
            ScriptButton = GetComponent<FlexibleButton>();
        }

        private void OnEnable()
        {
            //initial delay
            lastInputTime = Time.realtimeSinceStartup + 0.2f;
        }

        private void Update()
        {
            if (locked)
            {
                return;
            }
            Gamepad gamepad = Gamepad.current;
            if (gamepad is null || !inputManager.GamepadConnected)
            {
                return;
            }
            
            GamepadButton button = GamepadButton;
            if (inputManager.GamepadType == GamepadType.Switch)
            {
                if (GamepadButton == GamepadButton.South)
                {
                    button = GamepadButton.East;
                }
                else if (GamepadButton == GamepadButton.East)
                {
                    button = GamepadButton.South;
                }
            }
            
            if (gamepad.buttonEast.wasPressedThisFrame & button == GamepadButton.East || 
                gamepad.buttonWest.wasPressedThisFrame & button == GamepadButton.West ||
                gamepad.buttonNorth.wasPressedThisFrame & button == GamepadButton.North ||
                gamepad.buttonSouth.wasPressedThisFrame & button == GamepadButton.South)
            {
                if (Time.realtimeSinceStartup - lastInputTime < 0.3f)
                {
                    return;
                }
                lastInputTime = Time.realtimeSinceStartup;
                ScriptButton.Activate();
            }
        }

        public void Lock(bool on)
        {
            locked = on;
            //delay to prevent input hanging from another layer
            lastInputTime = Time.realtimeSinceStartup + 0.2f;
        }

        public bool IsLocked() => locked;
    }
}