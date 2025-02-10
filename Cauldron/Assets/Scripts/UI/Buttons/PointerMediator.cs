using UnityEngine;
using UnityEngine.EventSystems;

namespace Universal
{
    public class PointerMediator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private AnimatedButtonContainer _animatedButton;

        private void Awake()
        {
            _animatedButton = GetComponent<AnimatedButtonContainer>();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!_animatedButton.IsInteractive) return;

            _animatedButton.Selected();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!_animatedButton.IsInteractive) return;

            _animatedButton.Unselected();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {            
            if (!_animatedButton.IsInteractive) return;

            _animatedButton.Activate();
        }
    }
}