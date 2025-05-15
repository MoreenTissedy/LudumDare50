using System;
using System.Linq;
using CauldronCodebase;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Buttons
{
    public enum SelectableDirection
    {
        Horizontal,
        Vertical
    }

    public class SelectablesHolder : Selectable, IOverlayElement
    {
        [ReorderableList] public Selectable[] selectables;
        public SelectableDirection direction;
        
        public int startIndex = 0;

        private bool locked;
        private float lastInputTime;
        private int currentIndex = -1;

        public int CurrentIndex => currentIndex;
        private Selectable Current => currentIndex >= 0 ? selectables[currentIndex] : null;

        [Inject] private InputManager inputManager;

        private void Reset()
        {
            selectables = GetComponentsInChildren<Selectable>(false).Where(x => x != this).ToArray();
        }

        [Button("Clear")]
        public void Clear()
        {
            selectables = Array.Empty<Selectable>();
        }

        private void ActivateCurrent(InputAction.CallbackContext obj)
        {
            if (locked)
            {
                return;
            }
            Current.Activate();
        }

        private void Navigate(InputAction.CallbackContext context)
        {
            if (locked || Time.realtimeSinceStartup - lastInputTime < 0.3f)
            {
                return;
            }
            
            float diff = 0;
            if (direction == SelectableDirection.Vertical)
            {
                diff = - context.ReadValue<Vector2>().y;
            }

            if (direction == SelectableDirection.Horizontal)
            {
                diff = context.ReadValue<Vector2>().x;
            }

            if (diff < 0.8f && diff > -0.8f)
            {
                return;
            }

            if (diff > 0 && currentIndex == selectables.Length - 1)
            {
                return;
            }

            if (diff < 0 && currentIndex == 0)
            {
                return;
            }

            Current?.Unselect();
            lastInputTime = Time.realtimeSinceStartup;
            var oldIndex = currentIndex;
            currentIndex += diff > 0 ? 1 : -1;
            OnIndexChange(oldIndex, currentIndex);
            Current.Select();
        }

        public virtual void OnIndexChange(int oldIndex, int newIndex)
        {
            
        }

        public override void Select()
        {
            GetButtons();
            SelectDefaultElement();
            inputManager.Controls.General.NormalNavigate.performed += Navigate;
            inputManager.Controls.General.AnyKey.performed += ActivateCurrent;
        }

        private void GetButtons()
        {
            if (direction == SelectableDirection.Horizontal)
            {
                selectables = selectables.OrderBy(x => x.transform.position.x)
                    .Where(x => x.gameObject.activeInHierarchy).ToArray();
            }
            else if (direction == SelectableDirection.Vertical)
            {
                selectables = selectables.OrderByDescending(x => x.transform.position.y)
                    .Where(x => x.gameObject.activeInHierarchy).ToArray();
            }
        }

        protected virtual void SelectDefaultElement()
        {
            if (selectables is null || selectables.Length == 0) return;
            currentIndex = Mathf.Min(startIndex, selectables.Length - 1);
            Current.Select();
        }

        public override void Unselect()
        {
            Current?.Unselect();
            inputManager.Controls.General.NormalNavigate.performed -= Navigate;
            inputManager.Controls.General.AnyKey.performed -= ActivateCurrent;
        }

        public void Lock(bool on)
        {
            locked = on;
        }

        public bool IsLocked() => locked;
    }
}