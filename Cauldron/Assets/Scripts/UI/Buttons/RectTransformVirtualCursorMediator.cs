using CauldronCodebase;
using UnityEngine;
using UnityEngine.InputSystem;
using Universal;
using Zenject;

namespace UI.Buttons
{
    public class RectTransformVirtualCursorMediator: MonoBehaviour
    {
        private Camera uiCamera;
        private RectTransform rectTransform;
        private Mouse virtualMouse;
        private PointerMediator pointerMediator;

        private bool underCursor;
        
        [Inject] private InputManager inputManager;
        
        private void Start()
        {
            pointerMediator = GetComponent<PointerMediator>();
            pointerMediator.VirtualCursorControlable = true;
            
            virtualMouse = (Mouse)InputSystem.GetDevice("Virtual cursor");
            rectTransform = GetComponent<RectTransform>();
            uiCamera = GameObject.FindWithTag("UiCamera").GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (!inputManager.CursorEnabled)
            {
                return;
            }
            Vector2 mousePosition = virtualMouse.position.value;
            bool isOverlap = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition, uiCamera);
            
            if (isOverlap && !underCursor)
            {
                pointerMediator.OnPointerEnter(null);
            }
            else if (!isOverlap && underCursor)
            {
               pointerMediator.OnPointerExit(null);
            }

            underCursor = isOverlap;
            
            if (underCursor && virtualMouse.leftButton.isPressed)
            {
                pointerMediator.OnPointerClick(null);
            }
        }
    }
}