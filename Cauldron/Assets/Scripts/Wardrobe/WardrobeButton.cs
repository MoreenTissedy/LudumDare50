using CauldronCodebase.GameStates;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CauldronCodebase
{
    public class WardrobeButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private float offsetX = 5;
        [SerializeField] private float clickedOffset = -2;
        [SerializeField] private float moveDuration;
        private float initialXPos, offScreenXPos;
        
        [Inject] private Wardrobe wardrobe;
        [Inject] private GameDataHandler gameDataHandler;
        [Inject] private GameStateMachine stateMachine;
        [Inject] private SkinsProvider skinsProvider;

        private bool hidden;

        private void Awake()
        {
            initialXPos = transform.position.x;
            offScreenXPos = initialXPos + offsetX;
            gameObject.SetActive(false);
            
            stateMachine.OnGameStarted += TryShow;
            stateMachine.OnChangeState += Hide;
        }

        private void OnDestroy()
        {
            stateMachine.OnGameStarted -= TryShow;
            stateMachine.OnChangeState -= Hide;
            wardrobe.SkinApplied -= Hide;
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
        private void Show()
        {
            gameObject.SetActive(true);
            transform.DOLocalMoveX(initialXPos, moveDuration)
                .From(offScreenXPos)
                .SetEase(Ease.OutBack)
                .SetDelay(1.5f);
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
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (hidden)
            {
                return;
            }
            transform.DOLocalMoveX(initialXPos + clickedOffset, moveDuration);
            wardrobe.OpenWithCallback(Close);
        }
    }
}
