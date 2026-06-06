using System;
using System.Threading;
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

        private VirtualMouseInput virtualMouseInput;

        private CancellationTokenSource cts;

        public bool CursorEnabled => virtualMouseInput.CursorVisible;

        public InputManager(GameObject cursorFx, VirtualMouseInput virtualMouseInput)
        {
            Controls = new Controls();
            Controls.General.Enable();
            Controls.UI.Enable();
            
            GamepadConnected = Gamepad.current != null;
            //GamepadConnected = true;
            GamepadType = DetermineGamepadType(Gamepad.current);
            Debug.Log($"Gamepad Connected: {GamepadConnected} | Type: {GamepadType}");

            InputSystem.onDeviceChange += OnDeviceChange;
            this.virtualMouseInput = virtualMouseInput;
            
            virtualMouseInput.SetCursorVisible(false);
        }

        private void OnDeviceChange(InputDevice arg1, InputDeviceChange arg2)
        {
            if (Gamepad.current != null)
            {
                Debug.Log("Current gamepad: "+ (Gamepad.current.device ));
            }
            else
            {
                Debug.Log("Gamepad disconnected");
            }
            //InputChanged?.Invoke();
        }

        public async void SetCursor(bool enable)
        {
            if (CursorEnabled == enable)
            {
                return;
            }
            if (enable)
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), DelayType.Realtime, cancellationToken: cts.Token);
                Cursor.visible = true;
            }
            else if (GamepadConnected)
            {
                Cursor.visible = false;
            }

            virtualMouseInput.SetCursorVisible(enable);
        }

        private GamepadType DetermineGamepadType(Gamepad gamepad)
        {
            if (gamepad == null) 
                return GamepadType.None;

            if (gamepad is UnityEngine.InputSystem.DualShock.DualShockGamepad 
                || gamepad is UnityEngine.InputSystem.DualShock.DualSenseGamepadHID)
                return GamepadType.Playstation;

            if (gamepad is UnityEngine.InputSystem.XInput.XInputController)
                return GamepadType.XBox;

            if (gamepad is UnityEngine.InputSystem.Switch.SwitchProControllerHID)
                return GamepadType.Switch;

            return GamepadType.Unknown;
        }
    }
}