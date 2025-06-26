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
        public bool activateOnSelect = false;
        
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
            if (locked || Time.realtimeSinceStartup - lastInputTime < 0.3f)
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
            if (activateOnSelect)
            {
                Current.Activate();
            }
        }

        public virtual void OnIndexChange(int oldIndex, int newIndex)
        {
            
        }

        public override void Select()
        {
            GetButtons();
            SelectDefaultElement();
            inputManager.Controls.General.NormalNavigate.performed += Navigate;
            if (!activateOnSelect)
            {
                inputManager.Controls.General.AnyKey.performed += ActivateCurrent;
            }
            else
            {
                Current.Activate();
            }
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
            for (var index = 0; index < selectables.Length; index++)
            {
                var selectable = selectables[index];
                if (selectable.IsSelected())
                {
                    Debug.Log($"[Selectable {gameObject.name}] found selected on start: "+index);
                    SelectElement(index);
                    return;
                }
            }

            int startIndexVerified = Mathf.Min(startIndex, selectables.Length - 1);
            Debug.Log($"[Selectable {gameObject.name}] selected on start "+startIndexVerified);
            SelectElement(startIndexVerified);

            void SelectElement(int index)
            {
                currentIndex = index;
                Current.Select();
            }
        }

        public override void Unselect()
        {
            startIndex = currentIndex;
            Current?.Unselect();
            inputManager.Controls.General.NormalNavigate.performed -= Navigate;
            inputManager.Controls.General.AnyKey.performed -= ActivateCurrent;
        }

        public override bool IsSelected()
        {
            if (selectables is null || selectables.Length == 0) return false;
            foreach (var selectable in selectables)
            {
                if (selectable.IsSelected())
                {
                    return true;
                }
            }

            return false;
        }

        public void Lock(bool on)
        {
            locked = on;
            //delay to prevent input hanging from another layer
            lastInputTime = Time.realtimeSinceStartup + 0.2f;
        }

        public bool IsLocked() => locked;
    }
}