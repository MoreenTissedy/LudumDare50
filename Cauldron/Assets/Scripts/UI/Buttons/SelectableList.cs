using System;
using CauldronCodebase;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Buttons
{
    public class SelectableList: Selectable
    {
        public string[] Values;
        
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Color highlightColor;
        
        [Inject] private InputManager inputManager;

        private int currentIndex = -1;

        public event Action<int> OnValueChanged;

        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                if (Values is null || Values.Length == 0 || currentIndex == value)
                {
                    return;
                }
                currentIndex = Mathf.Clamp(0, Values.Length - 1, value);
                valueText.text = Values[currentIndex];
                OnValueChanged?.Invoke(currentIndex);
            }
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
            var diff = context.ReadValue<Vector2>().x;
            if (Mathf.Abs(diff) < 0.8f)
            {
                return;
            }
            CurrentIndex = diff < 0 ? CurrentIndex-- : CurrentIndex++;
        }

        public override void Unselect()
        {
            label.color = initialColor;
            inputManager.Controls.General.NormalNavigate.performed -= Navigate;
        }
    }
}