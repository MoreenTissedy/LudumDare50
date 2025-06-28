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
        public Selectable[] Selectables;

        public int CurrentIndex => currentIndex;
        private Selectable Current => currentIndex >= 0 ? Selectables[currentIndex] : null;

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

            if (TryActivate())
            {
                Debug.LogError($"[Selectable {gameObject.name}] activate current");
                Current.Activate();
            }
        }

        protected virtual bool TryActivate()
        {
            if (Current is SelectablesHolder)
            {
                return false;
            }
            return true;
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
                diff = -context.ReadValue<Vector2>().y;
            }

            if (direction == SelectableDirection.Horizontal)
            {
                diff = context.ReadValue<Vector2>().x;
            }

            if (diff < 0.8f && diff > -0.8f)
            {
                return;
            }

            if (diff > 0 && currentIndex == Selectables.Length - 1)
            {
                return;
            }

            if (diff < 0 && currentIndex == 0)
            {
                return;
            }

            int oldIndex = currentIndex;
            int newIndex = currentIndex + (diff > 0 ? 1 : -1);

            if (TryChangeIndex(oldIndex, newIndex))
            {
                Current?.Unselect();
                currentIndex = newIndex;
                Current.Select();

                lastInputTime = Time.realtimeSinceStartup;

                if (activateOnSelect)
                {
                    Current.Activate();
                }
            }
        }

        protected virtual bool TryChangeIndex(int oldIndex, int newIndex)
        {
            return true;
        }

        public override void Select()
        {
            GetActiveButtons();
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

        private void GetActiveButtons()
        {
            Selectables = selectables.Where(x => x.gameObject.activeInHierarchy).ToArray();
            Selectables = selectables.Where(x => x.gameObject.activeInHierarchy).ToArray();
        }

        protected virtual void SelectDefaultElement()
        {
            if (Selectables is null || Selectables.Length == 0) return;
            for (var index = 0; index < Selectables.Length; index++)
            {
                var selectable = Selectables[index];
                if (selectable.IsSelected())
                {
                    SelectElement(index);
                    return;
                }
            }

            int startIndexVerified = Mathf.Min(startIndex, Selectables.Length - 1);
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
            if (Selectables is null || Selectables.Length == 0) return false;
            foreach (var selectable in Selectables)
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