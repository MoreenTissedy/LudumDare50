using CauldronCodebase;
using UnityEngine;
using Zenject;

namespace Buttons
{
    public class ActivateByInput: MonoBehaviour
    {
        [Inject]
        private InputManager inputManager;

        public bool VisibleWithGamepad;

        private void Awake()
        {
            gameObject.SetActive(inputManager.GamepadConnected ^ !VisibleWithGamepad);
            inputManager.InputChanged += InputChanged;
        }

        private void InputChanged(GamepadType gamepadType)
        {
            gameObject.SetActive(gamepadType == GamepadType.None ^ VisibleWithGamepad);
        }

        private void OnDestroy()
        {
            inputManager.InputChanged -= InputChanged;
        }
    }
}