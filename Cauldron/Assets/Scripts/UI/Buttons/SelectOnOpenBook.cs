using System;
using CauldronCodebase;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Buttons
{
    public class SelectOnOpenBook: MonoBehaviour
    {
        public Book book;
        [Inject]
        private InputManager inputManager;

        private float initialDelay = 0.2f;

        private void Start()
        {
            book.OnOpen += Enable;
            book.OnClose += Disable;
        }

        private async void Enable()
        {
            if (inputManager.GamepadConnected)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(initialDelay), DelayType.Realtime);
                GetComponent<Selectable>()?.Select();
            }
        }

        private void Disable()
        {
            GetComponent<ISelectable>().Unselect();
        }

        private void OnDestroy()
        {
            book.OnOpen -= Enable;
            book.OnClose -= Disable;
        }
    }
}