using System;
using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Universal;
using Zenject;
using static UnityEngine.InputSystem.InputAction;

namespace CauldronCodebase
{
    public class WardrobeButton : FlexibleButton, IOverlayElement
    {
        [SerializeField] private float offsetX = 5;
        [SerializeField] private float clickedOffset = -2;
        [SerializeField] private float moveDuration;
        [SerializeField] private OverlayLayer overlayLayer;
        [SerializeField] private GameObject gamepadHint;
        
        private float initialXPos, offScreenXPos;
        
        [Inject] private Wardrobe wardrobe;
        [Inject] private GameDataHandler gameDataHandler;
        [Inject] private GameStateMachine stateMachine;
        [Inject] private SoundManager soundManager;
        [Inject] private InputManager inputManager;

        private bool hidden;
        private bool locked;

        private void Start()
        {
            initialXPos = transform.position.x;
            offScreenXPos = initialXPos + offsetX;
            gameObject.SetActive(false);
            
            stateMachine.OnGameStarted += TryShow;
            stateMachine.OnChangeState += Hide;
            
            overlayLayer.Register(this);

            inputManager.Controls.General.OpenWardrobe.performed += OnOpenWardrobePerfrormed;
        }

        private void OnDestroy()
        {
            stateMachine.OnGameStarted -= TryShow;
            stateMachine.OnChangeState -= Hide;
            wardrobe.SkinApplied -= Hide;
            inputManager.Controls.General.OpenWardrobe.performed -= OnOpenWardrobePerfrormed;
        }
        
        private void TryShow()
        {
            if (gameDataHandler.IsWardrobeButtonAvailable())
            {
                Show();
                wardrobe.SkinApplied += Hide;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        [Button("Show")]
        private async void Show()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            gameObject.SetActive(true);
            soundManager.Play(Sounds.WardrobeMove);
            await transform.DOLocalMoveX(initialXPos, moveDuration)
                .From(offScreenXPos)
                .SetEase(Ease.OutBack).ToUniTask();
        }

        private void Hide(GameStateMachine.GamePhase phase)
        {
            Hide();
        }
        
        private void Hide(SkinSO skin)
        {
            Hide();
        }

        [Button()]
        private void Hide()
        {
            soundManager.Play(Sounds.WardrobeMove);
            hidden = true;
            transform.DOLocalMoveX(offScreenXPos, moveDuration).SetEase(Ease.InBack)
                .OnComplete(()=> Destroy(gameObject));
        }

        public void Close()
        {
            if (hidden)
            {
                return;
            }
            transform.DOLocalMoveX(initialXPos, moveDuration);
            gamepadHint.SetActive(true);
        }
        
        public override void Activate()
        {
            if (hidden || locked)
            {
                return;
            }
            base.Activate();
            transform.DOLocalMoveX(initialXPos + clickedOffset, moveDuration);
            wardrobe.OpenWithCallback(Close);
            gamepadHint.SetActive(false);
        }

        public void Lock(bool on)
        {
            locked = on;
        }

        public bool IsLocked() => locked;

        private void OnOpenWardrobePerfrormed(CallbackContext context)
        {
            if (!gameObject.activeInHierarchy || hidden || locked)
                return;

            Activate();
        }
    }
}
