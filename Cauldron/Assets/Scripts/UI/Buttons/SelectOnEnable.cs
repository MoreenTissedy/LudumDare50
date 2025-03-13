using CauldronCodebase;
using UnityEngine;
using Zenject;

namespace Buttons
{
    public class SelectOnEnable: MonoBehaviour
    {
        [Inject]
        private InputManager inputManager;
        private void OnEnable()
        {
            if (inputManager.GamepadConnected)
            {
                GetComponent<ISelectable>()?.Select();
            }
        }

        private void OnDisable()
        {
            GetComponent<ISelectable>().Unselect();
        }
    }
}