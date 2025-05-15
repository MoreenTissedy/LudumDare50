using CauldronCodebase;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Buttons
{
    public class SelectableSlider: Selectable
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Color highlightColor;

        private int reduceInput = 100;
        
        [Inject] private InputManager inputManager;

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
            slider.value += diff / reduceInput;
        }

        public override void Unselect()
        {
            label.color = initialColor;
            inputManager.Controls.General.NormalNavigate.performed -= Navigate;
        }
    }
}