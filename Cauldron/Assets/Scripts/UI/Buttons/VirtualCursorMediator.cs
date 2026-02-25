using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace UI.Buttons
{
    public class VirtualCursorMediator: MonoBehaviour
    {
        public Collider2D collider;

        private Camera mainCamera;
        private Mouse virtualMouse;
        private IPointerEnterHandler pointerEnterHandler;
        private IPointerExitHandler pointerExitHandler;
        private IPointerClickHandler pointerClickHandler;

        private bool underCursor;
        
        private void Start()
        {
            pointerEnterHandler = GetComponent<IPointerEnterHandler>();
            pointerExitHandler = GetComponent<IPointerExitHandler>();
            pointerClickHandler = GetComponent<IPointerClickHandler>();
            virtualMouse = (Mouse)InputSystem.GetDevice("Virtual cursor");
        }

        private void LateUpdate()
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(virtualMouse.position.value);
            Debug.Log(worldPoint);
            bool isOverlap = collider.OverlapPoint(worldPoint);
            
            if (isOverlap && !underCursor)
            {
                Debug.LogError("Pointer enter "+gameObject.name);
                pointerEnterHandler?.OnPointerEnter(null);
            }
            else if (!isOverlap && underCursor)
            {
                Debug.LogError("Pointer exit "+gameObject.name);
                pointerExitHandler?.OnPointerExit(null);
            }

            underCursor = isOverlap;
            
            if (underCursor && virtualMouse.leftButton.isPressed)
            {
                Debug.LogError("Pointer click "+gameObject.name);
                pointerClickHandler?.OnPointerClick(null);
            }
        }
    }
}