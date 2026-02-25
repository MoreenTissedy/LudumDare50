using CauldronCodebase;
using UnityEngine;
using UnityEngine.EventSystems;
using NaughtyAttributes;
using Zenject;

namespace Universal
{
    public class PointerMediator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [ReadOnly]
        [SerializeField] private FlexibleButton _animatedButton;

        public bool VirtualCursorControlable;
        
        [Inject] InputManager inputManager;

        private void Reset()
        {
            _animatedButton = GetComponent<FlexibleButton>();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!VirtualCursorControlable && inputManager.GamepadConnected)
            {
                return;
            }
            if (!_animatedButton.IsInteractive) return;

            _animatedButton.Select();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!VirtualCursorControlable && inputManager.GamepadConnected)
            {
                return;
            }
            if (!_animatedButton.IsInteractive) return;

            _animatedButton.Unselect();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!VirtualCursorControlable && inputManager.GamepadConnected)
            {
                return;
            }
            if (!_animatedButton.IsInteractive) return;

            _animatedButton.Activate();
        }
    }
}