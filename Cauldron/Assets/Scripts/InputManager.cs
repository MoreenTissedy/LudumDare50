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

            UpdateGamepad();

            InputSystem.onDeviceChange += OnDeviceChange;
            this.virtualMouseInput = virtualMouseInput;
            
            virtualMouseInput.SetCursorVisible(false);
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change != InputDeviceChange.Added && change != InputDeviceChange.Removed)
                return;

            UpdateGamepad();
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

        private void UpdateGamepad()
        {
            GamepadConnected = Gamepad.current != null;
            GamepadType = DetermineGamepadType(Gamepad.current);
            Debug.Log($"Gamepad is connected: {GamepadConnected} | Type: {GamepadType}");

            InputChanged?.Invoke(GamepadType);
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