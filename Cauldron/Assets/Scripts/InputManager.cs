using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using Zenject;

namespace CauldronCodebase
{
    public enum GamepadType
    {
        None,
        XBox,
        Playstation,
        Switch,
        Unknown
    }

    public enum GamepadButton
    {
        None,
        East = 1,
        West = 2,
        South = 3,
        North = 4
    }
    public class InputManager
    {
        public readonly Controls Controls;

        public bool GamepadConnected;
        public GamepadType GamepadType;

        public Action<GamepadType> InputChanged;

        [Inject]
        private VirtualMouseInput virtualMouseInput; 
        
        public InputManager()
        {
            Controls = new Controls();
            Controls.General.Enable();
            Controls.UI.Enable();
            
            //GamepadConnected = Gamepad.current != null;
            GamepadConnected = true;
            GamepadType = GamepadType.Switch;
            Debug.LogError("Current gamepad: "+ (Gamepad.current?.device.ToString() ?? "none"));
            
            //todo: do not forget nonlazy
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        private void OnDeviceChange(InputDevice arg1, InputDeviceChange arg2)
        {
            if (Gamepad.current != null)
            {
                Debug.LogError("Current gamepad: "+ (Gamepad.current.device ));
            }
            else
            {
                Debug.LogError("Gamepad disconnected");
            }
            //InputChanged?.Invoke();
        }

        public async void SetCursor(bool enable)
        {
            if (enable)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), DelayType.Realtime);
                Cursor.visible = true;
            }
            else if (GamepadConnected)
            {
                Cursor.visible = false;
            }

            Debug.Log("[Set Cursor] "+enable);
            virtualMouseInput.enabled = enable;
        }
    }
}