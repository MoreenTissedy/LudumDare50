using System;
using CauldronCodebase;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Buttons
{
    public class SelectableList : Selectable, IOverlayElement
    {
        public string[] Values;

        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Color highlightColor;

        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;

        [Inject] private InputManager inputManager;

        private int currentIndex = -1;
        private float lastInputTime;
        private bool locked;

        public event Action<int> OnValueChanged;

        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                if (Values is null || Values.Length == 0 || currentIndex == value)
                {
                    return;
                }

                currentIndex = Mathf.Clamp(value, 0, Values.Length - 1);
                valueText.text = Values[currentIndex];
                OnValueChanged?.Invoke(currentIndex);
            }
        }

        private void Awake()
        {
            if (leftButton != null)
                leftButton.onClick.AddListener(SelectPrevious);
            if (rightButton != null)
                rightButton.onClick.AddListener(SelectNext);
        }

        private void OnDestroy()
        {
            if (leftButton != null) 
                leftButton.onClick.RemoveListener(SelectPrevious);
            if (rightButton != null) 
                rightButton.onClick.RemoveListener(SelectNext);
        }

        public void SetValueWithoutNotify(int index)
        {
            currentIndex = index;
            valueText.text = Values[currentIndex];
        }

        private Color initialColor;

        public override void Select()
        {
            initialColor = label.color;
            label.color = highlightColor;
            inputManager.Controls.General.NormalNavigate.performed += Navigate;
        }

        private void Navigate(InputAction.CallbackContext context)
        {
            if (locked || Time.realtimeSinceStartup - lastInputTime < 0.3f)
            {
                return;
            }

            var diff = context.ReadValue<Vector2>().x;
            if (Mathf.Abs(diff) < 0.8f)
            {
                return;
            }

            CurrentIndex = diff < 0 ? currentIndex - 1 : currentIndex + 1;
            lastInputTime = Time.realtimeSinceStartup;
        }

        public override void Unselect()
        {
            label.color = initialColor;
            inputManager.Controls.General.NormalNavigate.performed -= Navigate;
        }

        public void SelectPrevious()
        {
            if (locked) 
                return;
            CurrentIndex = currentIndex - 1;
        }

        public void SelectNext()
        {
            Debug.Log("Selecting next, locked: " + locked);
            if (locked) 
                return;
            CurrentIndex = currentIndex + 1;
        }

        public void Lock(bool on)
        {
            locked = on;
        }

        public bool IsLocked() => locked;
    }
}