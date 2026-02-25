using System;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Zenject;

namespace UI.Buttons
{
    public class VirtualCursorMediator: MonoBehaviour
    {
        private Collider2D theCollider;
        private Camera mainCamera;
        private Mouse virtualMouse;
        private IPointerEnterHandler pointerEnterHandler;
        private IPointerExitHandler pointerExitHandler;
        private IPointerClickHandler pointerClickHandler;

        private bool underCursor;

        [Inject] private InputManager inputManager;
        
        private float lastClickTime;
        
        private void Start()
        {
            pointerEnterHandler = GetComponent<IPointerEnterHandler>();
            pointerExitHandler = GetComponent<IPointerExitHandler>();
            pointerClickHandler = GetComponent<IPointerClickHandler>();
            virtualMouse = (Mouse)InputSystem.GetDevice("Virtual cursor");
            theCollider = GetComponent<Collider2D>();
        }

        private void LateUpdate()
        {
            if (!inputManager.CursorEnabled)
            {
                if (underCursor)
                {
                    pointerExitHandler?.OnPointerExit(null);
                    underCursor = false;
                }
                return;
            }
            if (!mainCamera)
            {
                mainCamera = Camera.main;
                return;
            }
            
            var worldPoint = mainCamera.ScreenToWorldPoint(virtualMouse.position.value);
            bool isOverlap = theCollider.OverlapPoint(worldPoint);
            
            if (isOverlap && !underCursor)
            {
                pointerEnterHandler?.OnPointerEnter(null);
            }
            else if (!isOverlap && underCursor)
            {
                pointerExitHandler?.OnPointerExit(null);
            }

            underCursor = isOverlap;
            
            if (underCursor && virtualMouse.leftButton.isPressed)
            {
                if (Time.realtimeSinceStartup - lastClickTime < 0.3f)
                {
                    return;
                }
                lastClickTime = Time.realtimeSinceStartup;
                pointerClickHandler?.OnPointerClick(null);
            }
        }
    }
}