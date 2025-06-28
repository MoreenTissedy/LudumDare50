using System;
using System.Threading;
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
        private CancellationTokenSource cts;
        private async void OnEnable()
        {
            if (inputManager.GamepadConnected)
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();
                await UniTask.Delay(TimeSpan.FromSeconds(initialDelay), DelayType.Realtime, cancellationToken: cts.Token);
                GetComponent<Selectable>()?.Select();
            }
        }

        private void OnDisable()
        {
            cts?.Cancel();
            GetComponent<ISelectable>().Unselect();
        }
    }
}