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
        [SerializeField] private RectTransform rect;
        [SerializeField] private float moveDuration;
        private float initialXPos, offScreenXPos;
        
        [Inject] private Wardrobe wardrobe;
        [Inject] private GameDataHandler gameDataHandler;
        [Inject] private GameStateMachine stateMachine;

        private void Awake()
        {
            initialXPos = rect.anchoredPosition.x;
            offScreenXPos = initialXPos + 300;
            rect.gameObject.SetActive(false);
            
            stateMachine.OnGameStarted += TryShow;
            stateMachine.OnChangeState += Hide;
        }

        private void OnDestroy()
        {
            stateMachine.OnGameStarted -= TryShow;
            stateMachine.OnChangeState -= Hide;
        }
        
        private void TryShow()
        {
            if (gameDataHandler.currentDay == 0 && gameDataHandler.cardsDrawnToday == 0)
            {
                Show();
            }
            else
            {
                Destroy(rect.gameObject);
            }
        }
        
        [Button]
        private void Show()
        {
            rect.gameObject.SetActive(true);
            rect.DOLocalMoveX(initialXPos, moveDuration)
                .From(offScreenXPos)
                .SetEase(Ease.OutBack)
                .SetDelay(1.5f);
        }

        [Button]
        private void Hide(GameStateMachine.GamePhase phase)
        {
            rect.DOLocalMoveX(offScreenXPos, moveDuration).SetEase(Ease.InBack)
                .OnComplete(()=> Destroy(rect.gameObject));
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            wardrobe.OpenBook();
        }
    }
}
