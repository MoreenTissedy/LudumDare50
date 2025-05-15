using System;
using CauldronCodebase;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Buttons
{
    public class SelectOnEnable: MonoBehaviour
    {
        [Inject]
        private InputManager inputManager;

        private float initialDelay = 0.2f;
        private async void OnEnable()
        {
            if (inputManager.GamepadConnected)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(initialDelay), DelayType.Realtime);
                GetComponent<ISelectable>()?.Select();
            }
        }

        private void OnDisable()
        {
            GetComponent<ISelectable>().Unselect();
        }
    }
}