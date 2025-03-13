using UnityEngine;
using UnityEngine.EventSystems;
using NaughtyAttributes;

namespace Universal
{
    public class PointerMediator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [ReadOnly]
        [SerializeField] private FlexibleButton _animatedButton;

        private void Reset()
        {
            _animatedButton = GetComponent<FlexibleButton>();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!_animatedButton.IsInteractive) return;

            _animatedButton.Select();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!_animatedButton.IsInteractive) return;

            _animatedButton.Unselect();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {            
            if (!_animatedButton.IsInteractive) return;

            _animatedButton.Activate();
        }
    }
}