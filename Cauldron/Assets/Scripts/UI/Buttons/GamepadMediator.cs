using System;
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
            
            if (gamepad.buttonEast.wasPressedThisFrame & GamepadButton == GamepadButton.East || 
                gamepad.buttonWest.wasPressedThisFrame & GamepadButton == GamepadButton.West ||
                gamepad.buttonNorth.wasPressedThisFrame & GamepadButton == GamepadButton.North ||
                gamepad.buttonSouth.wasPressedThisFrame & GamepadButton == GamepadButton.South)
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
        }

        public bool IsLocked() => locked;
    }
}